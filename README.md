<h1 align="center">HD2D SEMESTER PROJECT</h1>

<p align="center">
  <img src="https://img.shields.io/badge/STATUS-IN_PROGRESS-yellow?style=flat-square">
  <img src="https://img.shields.io/badge/MOTEUR-UNITY-white?style=flat-square">
</p>

---

## PRESENTATION DU PROJET
Prototype technique réalisé à **Rubika Valenciennes** focalisé sur la création d'un contrôleur de personnage haute performance pour un environnement HD2D. L'architecture est pensée pour être entièrement modulaire et facilement équilibrable par un Game Designer.

---

## ARCHITECTURE LOGICIELLE

### 1. Finite State Machine (FSM) avec Context
Le projet utilise une machine à états finis où chaque comportement (Locomotion, Dash, Carry, Parry, Attack) est encapsulé. 
* **Découplage et Évolutivité** : Isoler la logique dans des classes d'états permet d'ajouter de nouvelles fonctionnalités (ex: Grimper, Nager) sans risquer de briser les systèmes existants.
* **Gestion des Cycles de Vie** : Chaque état possède ses propres méthodes `EnterState` (initialisation), `UpdateState` / `FixedUpdateState` (logique continue) et `ExitState` (nettoyage), garantissant des transitions fluides et sans bugs de résidus.
* **Héritage intelligent** : Utilisation de classes de base comme `PlayerInAirBase` pour mutualiser la logique physique du saut, de la chute et du "bump".

### 2. Rôle du PlayerManager (The Orchestrator)
Le `PlayerManager` agit comme le chef d'orchestre du système. Plutôt que d'avoir un script massif, il se contente de :
* Gérer les transitions entre les états.
* Appeler dynamiquement l'Update et le FixedUpdate de l'état **actuel**.
* Cela permet de garder un composant principal extrêmement "clean" et facile à maintenir.

### 3. Design Orienté Données (ScriptableObjects)
Le système est **Data-Driven**. Grâce à `PlayerData.cs`, toutes les variables de gameplay sont centralisées dans des ScriptableObjects :
* **Bénéfice technique** : Modification du comportement en temps réel pendant le playtest. Les données sont sécurisées via des Wrappers (`PlayerDataInstance`) pour éviter toute corruption accidentelle des assets source.

---

## LOGIQUE DE COMBAT ET SENSATIONS (GAME FEEL)
* **Système de Parade Frame-Perfect** : Implémentation d'une double fenêtre de détection (Parry classique vs Perfect Parry) gérée par des coroutines pour une précision chirurgicale.
* **Conservation de Momentum** : Le `PlayerDashState` utilise un système de `velocityStock` pour conserver l'inertie du joueur en sortie d'action, garantissant une fluidité de mouvement.
* **Système d'Animation Robuste** : Utilisation de `BaseAnimationManager` avec des hashs `StringToHash` pour optimiser les performances.

---

## DIAGRAMME DE STRUCTURE (UML)
Voici comment les données et la logique s'articulent :

<img width="1061" height="1101" alt="HD2D_plan drawio" src="https://github.com/user-attachments/assets/5a7651b9-f72e-4a79-8312-1edbb9112b0a" />








