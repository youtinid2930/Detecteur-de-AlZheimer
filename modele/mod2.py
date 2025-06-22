# -*- coding: utf-8 -*-
import sys
import joblib

# R�cup�re le texte des sympt�mes pass� en argument
texte = sys.argv[1]
currentPath = sys.argv[2]

# Charge le mod�le
model = joblib.load(f"{currentPath}/modele/modele_alzheimer1.joblib")

# Fait la pr�diction
pred = model.predict([texte])[0]
confiance = model.predict_proba([texte])[0][pred]
label = "Alzheimer" if pred == 1 else "Non"

# Affiche le r�sultat pour que .NET puisse le lire
print(f"{label}|{confiance:.2f}")
