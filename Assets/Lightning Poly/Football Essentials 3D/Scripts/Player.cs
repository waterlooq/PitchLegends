namespace LightningPoly.FootballEssentials3D
{
    using UnityEngine;

    public class Player : MonoBehaviour
    {

         public GameObject[] decorations, eyes, mouths, hairs, all;

        private void OnGUI()
        {
            if (GUILayout.Button("Change Character Appearance"))
            {
                ChangeCloth();
            }
        }
        [ContextMenu(nameof(ChangeCloth))]
        public void ChangeCloth()
        {
            foreach (var item in all)
            {
                item.SetActive(false);
            }
            decorations[Random.Range(0, decorations.Length)].SetActive(true);
            eyes[Random.Range(0, eyes.Length)].SetActive(true);
            mouths[Random.Range(0, mouths.Length)].SetActive(true);
            hairs[Random.Range(0, hairs.Length)].SetActive(true);
        }


        void Start()
        {

        }

        
    }

}

