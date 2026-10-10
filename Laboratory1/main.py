import cv2

face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
if face_cascade.empty():
    face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')

if face_cascade.empty():
    print('Не вдалося завантажити каскад для обличчя.')
    exit()

eyes_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_eye.xml')
if eyes_cascade.empty():
    print('Не вдалося завантажити каскад для очей.')
    exit()

smile_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_smile.xml')
if smile_cascade.empty():
    print('Не вдалося завантажити каскад для посмішки.')
    exit()


def detect_faces_and_eyes(image, gray_image):
    faces = face_cascade.detectMultiScale(gray_image, scaleFactor=1.1, minNeighbors=5, minSize=(30, 30))

    for (x, y, w, h) in faces:
        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)
        cv2.putText(image, 'Face', (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)

        face_roi_gray = gray_image[y:y + h, x:x + w]
        face_roi_color = image[y:y + h, x:x + w]

        eyes = eyes_cascade.detectMultiScale(face_roi_gray, scaleFactor=1.1, minNeighbors=10, minSize=(15, 15))
        for (ex, ey, ew, eh) in eyes:
            cv2.rectangle(face_roi_color, (ex, ey), (ex + ew, ey + eh), (255, 0, 0), 2)
            cv2.putText(face_roi_color, 'Eye', (ex, ey - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)

        smiles = smile_cascade.detectMultiScale(face_roi_gray, scaleFactor=1.7, minNeighbors=20, minSize=(25, 25))
        for (sx, sy, sw, sh) in smiles:
            cv2.rectangle(face_roi_color, (sx, sy), (sx + sw, sy + sh), (0, 0, 255), 2)
            cv2.putText(face_roi_color, 'Smile', (sx, sy - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 2)

    return faces

photo_path = 'Tom.jpg'
img = cv2.imread(photo_path)

if img is not None:
    gray_img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    gray_img = cv2.equalizeHist(gray_img)

    faces = detect_faces_and_eyes(img, gray_img)
    print(f'Знайдено облич на фото: {len(faces)}')

    cv2.imshow('Lab 1 - Photo Detection', img)
    print('Натисніть будь-яку клавішу, щоб перейти до відео')
    cv2.waitKey(0)
    cv2.destroyAllWindows()
else:
    print(f"Фото '{photo_path}' не знайдено")

cap = cv2.VideoCapture(0)

if not cap.isOpened():
    print('Не вдалося отримати доступ до веб-камери.')
    exit()

print("Запуск відеопотоку. Натисніть клавішу 'q', щоб вийти")

while True:
    ret, frame = cap.read()
    if not ret:
        print('Не вдалося отримати кадр з камери.')
        break

    gray_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    gray_frame = cv2.equalizeHist(gray_frame)

    detect_faces_and_eyes(frame, gray_frame)

    cv2.imshow('Lab 1 - Real-time Video Detection', frame)

    key = cv2.waitKey(1) & 0xFF
    if key == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
print('Завершено.')