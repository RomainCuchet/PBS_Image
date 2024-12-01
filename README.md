
# Projet Scientifique Informatique

Ce projet a été réaliser dans le cadre d'un projet imposer par l'ESILV. Il explore les différentes étapes de manipulation d'images numériques, depuis leur représentation en mémoire jusqu'à des opérations comme la rotation d'images, la stéganographie et la génération de fractales.

  

## Table des Matières

1. [Format Pivot](#conversion-jpeg)

2. [Conversion JPEG](#conversion-jpeg)

3. [Enregistrement JPEG](#enregistrement-jpeg)

4. [Interpolation Bilinéaire](#interpolation-bilinéaire)

5. [Lecture des Images avec Bits de Remplissage](#lecture-des-images-avec-bits-de-remplissages)

6. [Stéganographie](#stéganographie)

  

## Format Pivot

  

Nous représentons les images sous la forme d’une matrice de pixels, un format standard dans le traitement d’images qui permet :

- Une manipulation intuitive des données.

- Une intégration facilitée avec des algorithmes de traitement d'image documentés.

Nous avons intégré une classe Pixel, enrichie de méthodes spécifiques, pour simplifier les manipulations et rendre les fonctions plus modulaires et lisibles.

  

## Conversion JPEG

  

Pour la conversion au format JPEG, nous utilisons trois matrices distinctes de type double :

  

- Y (luminosité),

- Cb (chrominance bleue),

- Cr (chrominance rouge).

  

### Justifications :

- Précision accrue : Les calculs nécessaires à la Discrete Cosine Transform (DCT) et la quantification utilisent des opérations en virgule flottante (sqrt, divisions, etc.). Travailler directement en double réduit les erreurs d'arrondi.

- Modularité : Travailler avec trois matrices distinctes améliore la clarté du code et permet de manipuler indépendamment chaque composante du modèle YCbCr.

- Facilité de stockage : Bien que nous n’ayons pas encore réussi à sauvegarder des images directement en format JPEG, ce découpage en matrices distinctes simplifie le processus d’intégration future.

  

source : [JPEG — Wikipédia (wikipedia.org)](https://fr.wikipedia.org/wiki/JPEG) et les pages Wikipédia associées aux étapes.

  

## Enregistrement JPEG

  

Pour écrire les images au format JPEG, nous utilisons une structure struct dédiée au header, composée de tableaux de bytes pour représenter les différents segments (SOF0, APP0, DHT, etc.).

  

- Méthodes set_...() configurant chaque segment avant l’écriture.

- Gestion simplifiée des données de fin de fichier (EOI) via un tableau de bytes.

- Approche modulaire facilitant les mises à jour ou les extensions futures.

- Compatibilité avec les standards définis par les spécifications JPEG.

  

Source : [https://youtu.be/sb8CQ9knDgI?si=wkZDP_XoRGeK8-Wh](https://youtu.be/sb8CQ9knDgI?si=wkZDP_XoRGeK8-Wh), [https://web.archive.org/web/20120403212223/http://class.ee.iastate.edu/ee528/Reading material/JPEG_File_Format.pdf](https://web.archive.org/web/20120403212223/http://class.ee.iastate.edu/ee528/Reading%20material/JPEG_File_Format.pdf)

  

## Interpolation bilinéaire

  

Lors des rotations ou transformations géométriques, chaque pixel transformé ne correspond pas toujours à une position exacte dans l'image d'origine. Pour une qualité optimale, nous implémentons une interpolation bilinéaire, qui calcule la valeur d’un pixel en fonction de ses quatre voisins les plus proches.

  

Cette méthode garantit un rendu visuellement cohérent, réduisant les artefacts liés aux transformations.

  

[https://www.iro.umontreal.ca/~mignotte/IFT6150/Chapitre7_IFT6150.pdf](https://www.iro.umontreal.ca/~mignotte/IFT6150/Chapitre7_IFT6150.pdf)

  

## Lecture des images avec bits de remplissages

  

Certaines images BMP incluent des bits de remplissage pour des raisons d’alignement matériel. Ces bits, qui augmentent artificiellement la taille du fichier, nécessitent une gestion particulière lors de la lecture d u fait de notre implémentation.

  

La méthode get_image() supprime automatiquement les bits de remplissage, en ajustant la taille des données lues.

  

Option désactivée par défaut : Certaines fonctions génèrent des erreurs avec cette prise en charge active.

  

[https://www.gladir.com/LEXIQUE/FICHIERS/bmp.htm](https://www.gladir.com/LEXIQUE/FICHIERS/bmp.htm)

  

## Stéganographie

  

Notre module de stéganographie permet d’encoder et de décoder des images cachées tout en préservant les couleurs de l’image hôte.

  

### Fonctionnalités :

1. Compression des bytes de l’image cachée sur un certain nombre de bits pour limiter les pertes de couleur.

2. Paramétrage flexible :

- Sélection des chaînes de couleur à utiliser (R, G, B).

- Quantité de bits par chaîne configurable.

3. Capacité multicanal : Possibilité de cacher jusqu'à trois images différentes dans une seule image hôte, bien qu'avec une perte d’informations colorimétriques.

## Références

1.  **JPEG** : [Wikipédia — JPEG](https://fr.wikipedia.org/wiki/JPEG)
2.  **Format JPEG** :
    -   [Vidéo explicative](https://youtu.be/sb8CQ9knDgI?si=wkZDP_XoRGeK8-Wh)
    -   [Structure du format JPEG](https://web.archive.org/web/20120403212223/http://class.ee.iastate.edu/ee528/Reading%20material/JPEG_File_Format.pdf)
3.  **Interpolation bilinéaire** : [Notes de cours](https://www.iro.umontreal.ca/~mignotte/IFT6150/Chapitre7_IFT6150.pdf)
4.  **Bits de remplissage BMP** : [Documentation Gladir](https://www.gladir.com/LEXIQUE/FICHIERS/bmp.htm)