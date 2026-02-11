using System;
using UnityEngine;

namespace com.Victor.Utilities.Scripts
{
    /// <summary>
    /// Fonctions utilitaires pour gérer les interactions avec les objets du jeu.
    /// </summary>
    public static class InteractionHelper
    {
        /// <summary>
        /// Détecte un objet interactif devant le joueur en utilisant un raycast.
        /// </summary>
        /// <typeparam name="T">Le type de composant recherché</typeparam>
        /// <param name="player">Le Transform du joueur</param>
        /// <param name="distance">La distance maximale de détection</param>
        /// <param name="layer">Le LayerMask pour filtrer les objets détectables</param>
        /// <returns>Le composant T si trouvé, sinon null/default</returns>
        public static T GetInteractableInFront<T>(Transform player, float distance, LayerMask layer)
        {
            return Physics.Raycast(player.position, player.forward,out RaycastHit hit,distance,layer ) 
                ? hit.collider.GetComponent<T>() : default;
        }

        /// <summary>
        /// Ajoute un effet de highlight (surbrillance) à un objet en activant l'émission du matériau.
        /// Nécessite un shader URP/Lit avec émission activée.
        /// </summary>
        /// <param name="obj">Le GameObject à illuminer</param>
        /// <param name="color">La couleur du highlight</param>
        /// <param name="intensity">L'intensité de l'émission</param>
        /// <exception cref="Exception">Si l'objet est null ou n'a pas de Renderer</exception>
        public static void HighlightObject(GameObject obj, Color color, float intensity)
        {
            if (obj == null) throw new Exception("GameObject null");

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) throw new Exception("No Renderer attached to this GO");

            Material material = renderer.material;

            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * intensity);
        }
        
        /// <summary>
        /// Retire l'effet de highlight d'un objet en désactivant l'émission du matériau.
        /// </summary>
        /// <param name="obj">Le GameObject dont retirer le highlight</param>
        /// <exception cref="Exception">Si l'objet est null ou n'a pas de Renderer</exception>
        public static void RemoveHighlight(GameObject obj)
        {
            if (obj == null) throw new Exception("GameObject null");

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) throw new Exception("No Renderer attached to this GO");

            Material material = renderer.material;

            material.DisableKeyword("_EMISSION");
        }

        /// <summary>
        /// Vérifie s'il existe une ligne de vue dégagée entre deux points.
        /// Utile pour l'IA ennemie ou les systèmes de visibilité.
        /// </summary>
        /// <param name="from">Le point de départ</param>
        /// <param name="to">Le point d'arrivée</param>
        /// <param name="obstacles">Le LayerMask des obstacles bloquant la vue</param>
        /// <returns>True si la ligne de vue est dégagée, False si un obstacle bloque</returns>
        public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacles)
        {
            Vector3 direction = to - from;
            return !Physics.Raycast(from, direction, direction.magnitude, obstacles);
        }
    }
}