using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.puzzles.Card_MiniGame
{
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        private int maxHealth;
        [SerializeField] private int currenthealth;
        [HideInInspector] public int attackDamage;
        private CardMinigameManager manager;

        [HideInInspector] public bool isDestroyed = false;
        [HideInInspector] public bool isInDeck = true;
        [HideInInspector] public bool hasSpecialEffect = false;

        public void InitCard(CardMinigameManager _manager, int _health, int _attackDamage, bool _hasSpecialEffect)
        {
            manager = _manager;
            attackDamage = _attackDamage;
            maxHealth = _health;
            hasSpecialEffect = _hasSpecialEffect;
        }

        public void ResetHp()
        {
            isInDeck = true;
            isDestroyed = false;
            currenthealth = maxHealth;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.CardSelected(this);
        }

        public void TakeDamage(int damage)
        {
            currenthealth -= damage;

            if(currenthealth <= 0)
            {
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            manager.RemoveHealth();
            gameObject.SetActive(false);

            isDestroyed = true;
        }
    }
}
