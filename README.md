# RogueMapVisualizer

RogueMapVisualizer est une application développée en C# et WPF permettant de visualiser la génération procédurale de cartes de type roguelike.

Le projet met en avant plusieurs étapes de génération :
- création du biome
- détection des clusters (BFS)
- connexion des zones (Kruskal)
- génération des rooms
- génération des murs

L'application permet d'afficher ces étapes progressivement et d'activer ou désactiver différentes couches (biome, rooms, murs, clusters) afin de mieux comprendre les algorithmes utilisés.

## Lancer l'application

1. Télécharger et extraire le fichier ZIP de la dernière release
2. Ouvrir le dossier `Release`
3. Lancer le fichier : RogueMapVisualizer.exe
