import sys
from predictor import AlzheimerPredictor
import os

def main():
    if len(sys.argv) < 3:
        print("Usage: app.py model_path class_path image_path")
        return

    model_path = sys.argv[1]
    class_path = sys.argv[2]
    image_path = sys.argv[3]
    
    with open(image_path, "rb") as f:
        file_bytes = f.read()

    content_type = "image/jpeg"

    predictor = AlzheimerPredictor(model_path, class_path)
    result = predictor.predict(file_bytes, content_type)

    print(f"Classe: {result['class']}")
    print(f"Confiance: {result['confidence']:.2f}%")

if __name__ == "__main__":
    main()
