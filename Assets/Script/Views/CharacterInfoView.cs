using TMPro;
using Script.RunTimeDatas;
using UnityEngine;

namespace Script.Views
{
    public class CharacterInfoView : MonoBehaviour
    {
        public TMP_Text characterExpNum;
        public PlayerRunTimeData playerRunTimeData;
        public static TMP_Text SCharacterExpNum;
        public static PlayerRunTimeData SPlayerRunTimeData;

        private void Awake()
        {
            SCharacterExpNum = characterExpNum;
            playerRunTimeData = FindObjectOfType<PlayerRunTimeData>();
            SPlayerRunTimeData = playerRunTimeData;
        }
    }
}