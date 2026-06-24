using TMPro;
using UnityEngine;

namespace Script.Views
{
    public class CharacterInfoView : MonoBehaviour
    {
        public  TMP_Text characterExpNum;
        public static TMP_Text SCharacterExpNum;

        private void Awake()
        {
            SCharacterExpNum = characterExpNum;
        }
    }
}
