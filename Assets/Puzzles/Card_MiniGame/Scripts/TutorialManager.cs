using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Card_MiniGame
{
    public class TutorialManager : MonoBehaviour, IPointerClickHandler
    {
        public CardMinigameManager m_CardMinigameManager;
        [SerializeField] private string[] sentences;
        [SerializeField] private TextMeshPro tmpro;

        [SerializeField] private Card playerCard;
        [SerializeField] private Card enemyCard;
        [SerializeField] private Transform glow;
        private int currentSentence = 0;
        private bool isAttacked = false;
        public void StartTutorial(CardMinigameManager _cardManager)
        {
            m_CardMinigameManager = _cardManager;
            m_CardMinigameManager.playerCards.Add(playerCard);
            m_CardMinigameManager.enemyCards.Add(enemyCard);
            playerCard.isInDeck = false;
            enemyCard.isInDeck = false;
            playerCard.gameObject.SetActive(true);
            enemyCard.gameObject.SetActive(true);
            playerCard.InitCard(_cardManager, 5, 5, false);
            enemyCard.InitCard(_cardManager, 2, 5, false);

            currentSentence = 0;
            isAttacked = false;
            ChangeText();
        }

        public void Attacked()
        {
            isAttacked = true;
        }
        private void ChangeText()
        {
            if (currentSentence >= sentences.Length)
            {
                TutorialDone();
                return;
            }
            tmpro.text = sentences[currentSentence];
            currentSentence++;

        }

        private void TutorialDone()
        {
            m_CardMinigameManager.isTutorial = false;
            m_CardMinigameManager.InitGame();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isAttacked)
                ChangeText();
        }
    }
}