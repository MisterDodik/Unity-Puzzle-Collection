using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using static com.puzzles.Card_MiniGame.GameData;


namespace com.puzzles.Card_MiniGame
{
    public class CardMinigameManager : MonoBehaviour
    {
        [SerializeField] private Transform puzzleParent;
        [SerializeField] private Transform upperSectionParent;
        [SerializeField] private Transform middleSectionParent;
        [SerializeField] private Transform bottomSectionParent;


        [SerializeField] private GameData GameData;

        [SerializeField] private GameObject faceDownCardPrefab;
        [SerializeField] private int initialFacedownCardCount;
        private List<Transform> spawnedCardDowns = new List<Transform>();

        [SerializeField] private Transform allCardsPrefab;
        private Transform allCards;
        private List<Card> pooledGameCards = new List<Card>();

        [SerializeField] private Transform cardGlow;

        //Game loop
        [HideInInspector] public bool isTutorial = true;
        [SerializeField] private GameObject tutorialGO;
        [SerializeField] private GameObject tutorialSpawned;
        [SerializeField] private TutorialManager tutorialManager;


        private int playerHealth;
        private int enemyHealth;
        [SerializeField] private TextMeshPro playerHealthTMPro;
        [SerializeField] private TextMeshPro enemyHealthTMPro;

        private bool playerMove = true;
        private List<Card> middleSectionCards = new List<Card>();

        [HideInInspector] public List<Card> playerCards = new List<Card>();

        private Queue<Card> enemyDeck = new Queue<Card>();
        [HideInInspector] public List<Card> enemyCards = new List<Card>();


        private void Start()
        {
            InitGame();
        }
        public void ResetGame()
        {
            InitGame();
        }
        public void Skip()
        {
            
        }

        public void Completed()
        {
            flagGameOver = true;
            EndGamePanel();
        }


        private void CustomGameOver(bool isWon)
        {
            eventHandler.SetActive(true);
            if (isWon)
            {
                endGameText.text = "You won";
            }
            else
            {
                endGameText.text = "You lost";
            }
            Completed();
        }


        public void InitGame()
        {
            cardGlow.gameObject.SetActive(false);

            playerHealth = 35;
            playerHealthTMPro.text = playerHealth.ToString();

            enemyHealth = 35;
            enemyHealthTMPro.text = enemyHealth.ToString();

            playerMove = true;
            initialCardsSelected = false;
            flagGameOver = false;
            isBusy = false;



            playerCards.Clear();
            enemyCards.Clear();
            middleSectionCards.Clear();
            enemyDeck.Clear();
            attackerCardSelected = null;
            oppositeCardSelected = null;
            turnCount = 0;

            if (isTutorial)
            {
                initialCardsSelected = true;
                if (!tutorialSpawned)
                {
                    tutorialSpawned = Instantiate(tutorialGO, puzzleParent);
                    tutorialSpawned.transform.localPosition = new Vector3(0, 1.5f, 0);
                    tutorialManager = tutorialSpawned.GetComponent<TutorialManager>();
                }

                tutorialSpawned.SetActive(true);
                tutorialManager.StartTutorial(this);

                return;
            }

            tutorialSpawned.SetActive(false);
            //pool playable karata

            if(allCards == null)
            {
                allCards = Instantiate(allCardsPrefab, puzzleParent);
            }

            if (pooledGameCards.Count == 0)
            {
                for (int i = 0; i < GameData.cardData.Count; i++) 
                {
                    CardData data = GameData.cardData[i];

                    Card card = allCards.GetChild(i).GetComponent<Card>();

                    card.InitCard(this, data.cardHp, data.AD, data.hasSpecialEffect);

                    card.gameObject.SetActive(false);

                    if (i >= pooledGameCards.Count)
                    {
                        pooledGameCards.Add(card);
                    }
                }

                for (int i = 0; i < 18 - pooledGameCards.Count; i++)
                {
                    int randIndex = Random.Range(0, GameData.cardData.Count);
                    CardData data = GameData.cardData[randIndex];
                    Transform prefabTransform = allCards.GetChild(randIndex);
                    Card duplicate = Instantiate(prefabTransform.gameObject, allCards).GetComponent<Card>();
                    duplicate.InitCard(this, data.cardHp, data.AD, data.hasSpecialEffect);
                    duplicate.gameObject.SetActive(false);
                    pooledGameCards.Add(duplicate);
                }
            }
            
            shuffleList(pooledGameCards);

            foreach(Card c in pooledGameCards)
            {
                c.gameObject.SetActive(false);
                c.ResetHp();
            }

            //pool facedown karata
            for (int i = 0; i<initialFacedownCardCount; i++)
            {
                Transform card;
                if(i>=spawnedCardDowns.Count)
                {
                    card = Instantiate(faceDownCardPrefab, upperSectionParent).transform;
                    spawnedCardDowns.Add(card);
                }
                else
                {
                    card = spawnedCardDowns[i];
                }
                card.gameObject.SetActive(false);
            }

            InitialCardSelection();
        }

        private void shuffleList(List<Card> cards)
        {
            var count = cards.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = cards[i];
                cards[i] = cards[r];
                cards[r] = tmp;
            }
        }
        private void InitialCardSelection()
        {
            //aktiviranje srednji karata
            for (int i = 0; i < 6; i++)
            {
                Card c = pooledGameCards[i];

                middleSectionCards.Add(c);
                c.isInDeck = false;

                int x = i % 3;
                int y = i > 2 ? 0 : 1;
                c.transform.SetParent(middleSectionParent);
                c.transform.localScale = new Vector3(1, 1, 1);
                c.transform.localPosition = GameData.centralSectionOrigin + new Vector2(GameData.cardSpacing.x * x, GameData.cardSpacing.y * y);

                c.gameObject.SetActive(true);
            }
        }

        int enemyNewCardIndex;
        private void ContinueLoadingCards()
        {
            cardGlow.gameObject.SetActive(false);

            foreach (Card c in middleSectionCards)
            {
                c.isInDeck = true;
                c.gameObject.SetActive(false);
            }

            //aktiviranje ovih gore karata
            for (int i = 0; i < initialFacedownCardCount; i++)
            {
                Transform c = spawnedCardDowns[i];

                c.transform.SetParent(upperSectionParent);
                c.transform.localScale = new Vector3(1, 1, 1);
                c.localPosition = GameData.upperSectionOrigin + new Vector2(GameData.cardSpacing.x * i, 0);

                c.gameObject.SetActive(true);
            }


            //aktiviranje srednji karata
            //enemy
            for (int i = 0, k = 0; i < middleSectionCards.Count; i++)
            {
                Card c = middleSectionCards[i];
                if (playerCards.Contains(c))
                    continue;

                c.isInDeck = false;
                enemyCards.Add(c);
                
                int x = k % 3;
                int y = 1;
                c.transform.SetParent(middleSectionParent);
                c.transform.localScale = new Vector3(1, 1, 1);
                c.transform.localPosition = GameData.centralSectionOrigin + new Vector2(GameData.cardSpacing.x * x, GameData.cardSpacing.y * y);

                c.gameObject.SetActive(true);
                k++;
            }
            //player
            for (int i = 0; i < playerCards.Count; i++)
            {
                Card c = playerCards[i];
                c.isInDeck = false;

                int x = i % 3;
                int y = 0;
                c.transform.SetParent(middleSectionParent);
                c.transform.localScale = new Vector3(1, 1, 1);
                c.transform.localPosition = GameData.centralSectionOrigin + new Vector2(GameData.cardSpacing.x * x, GameData.cardSpacing.y * y);

                c.gameObject.SetActive(true);
            }


            //aktiviranje onih dole karata
            for (int i = 6; i < initialFacedownCardCount+6; i++)
            {
                Card c = pooledGameCards[i];

                c.isInDeck = true;

                c.transform.SetParent(bottomSectionParent);
                c.transform.localScale = new Vector3(1, 1, 1);
                c.transform.localPosition = GameData.lowerSectionOrigin + new Vector2(GameData.cardSpacing.x * (i-6), 0);

                c.gameObject.SetActive(true);
            }

            enemyDeck.Clear();
            for (int i = 12; i < pooledGameCards.Count; i++)
            {
                Card c = pooledGameCards[i];
                c.isInDeck = true;
                enemyDeck.Enqueue(c);
            }
            turnCount = 0;
            enemyNewCardIndex = initialFacedownCardCount;
        }
        
        int turnCount = 0;
        bool initialCardsSelected = false;
        private Card attackerCardSelected;
        private Card oppositeCardSelected;
        private bool isBusy = false;
        public void CardSelected(Card card)
        {
            if (flagGameOver || isBusy)
                return;
            cardGlow.gameObject.SetActive(true);
            cardGlow.transform.localScale = card.transform.parent.localScale;
            cardGlow.transform.localPosition = card.transform.parent.TransformPoint(card.transform.localPosition);

            if (!initialCardsSelected)
            {
                if (playerCards.Contains(card))
                    return;
                card.isInDeck = false;
                playerCards.Add(card);
                turnCount++;

            }
            if (turnCount == 3 && !initialCardsSelected)
            {
                initialCardsSelected = true;
                ContinueLoadingCards();
            }
            if (!initialCardsSelected)
                return;

            if(card.isInDeck)
            {
                TakeNewCard(card, playerMove);
                return;
            }

            if (playerMove)
            {
                if (playerCards.Contains(card))
                    attackerCardSelected = card;                    
                else if (enemyCards.Contains(card) && attackerCardSelected!=null && oppositeCardSelected == null)
                    oppositeCardSelected = card;
            }

            if (oppositeCardSelected != null && attackerCardSelected != null)
            {
                isBusy = true;
                AttackMechanics();

                if (playerMove && !isTutorial)
                {
                    EnemyTurn();                 
                }
            }
        }

        bool flagGameOver = false;
        private void EnemyTurn()
        {
            StartCoroutine(enemyTurnAnimation());
        }
        
        private IEnumerator enemyTurnAnimation()
        {
            //disable mouse events
            eventHandler.SetActive(false);

            yield return new WaitForSeconds(1f);

            if (flagGameOver)
                yield break;

            playerMove = false;

            int count = enemyCards.Count;
            if(count < 2 && enemyNewCardIndex>=0 && enemyDeck.Count > 0)
            {
                Card newCard = enemyDeck.Dequeue();
                spawnedCardDowns[enemyNewCardIndex - 1].gameObject.SetActive(false);
                CardSelected(newCard);
            }

            Card attacker = enemyCards[Random.Range(0, enemyCards.Count)];
            Card victim = playerCards[Random.Range(0, playerCards.Count)];

            CardSelected(attacker);

            yield return new WaitForSeconds(0.2f);
            CardSelected(victim);

            attackerCardSelected = attacker;
            oppositeCardSelected = victim;

            yield return new WaitForSeconds(0.2f);
            AttackMechanics();      
        }
        
        private void AttackMechanics()
        {
            cardGlow.gameObject.SetActive(false);

            DoAttackAnimation(attackerCardSelected, oppositeCardSelected);
        }
        private void DoAttackAnimation(Card attacker, Card victim)
        {
            //disable mouse events
            eventHandler.SetActive(false);

            Vector2 victimPosition = victim.transform.localPosition;
            Vector2 attackerPosition = attacker.transform.localPosition;

            float distance = Vector2.Distance(victimPosition, attackerPosition);
            Vector2 direction = (victimPosition - attackerPosition).normalized * distance / 3;

            Vector2 finalPosition = attackerPosition + direction;

            attacker.transform.DOLocalMove(finalPosition, 0.5f).OnComplete(() =>
            {
                victim.TakeDamage(attacker.attackDamage);
                attacker.transform.localPosition = attackerPosition;

                if (oppositeCardSelected.isDestroyed)
                {
                    if (playerMove)
                        enemyCards.Remove(oppositeCardSelected);
                    else
                        playerCards.Remove(oppositeCardSelected);
                }

                oppositeCardSelected = null;
                attackerCardSelected = null;

               
                //enable mouse events
                if (!playerMove)
                    eventHandler.SetActive(true);

                playerMove = true;
                isBusy = false;


                //enable mouse events
                if (isTutorial)
                {
                    eventHandler.SetActive(true);
                    tutorialManager.Attacked();
                }
            });
        }

        public void RemoveHealth()
        {
            if (playerMove)
            {
                enemyHealth -= 5;
                enemyHealthTMPro.text = enemyHealth.ToString();
            }
            else
            {
                playerHealth -= 5;
                playerHealthTMPro.text = playerHealth.ToString();
            }

            if (enemyHealth <= 0)
            {
                CustomGameOver(true);
                return;
            }
            if (playerHealth <= 0)
            {
                CustomGameOver(false);
                return;
            }
        }

        private void TakeNewCard(Card card, bool isPlayerCard)
        {
            int yPosition;
            List<Card> cards;
            if (isPlayerCard)
            {
                cards = playerCards;
                yPosition = 0;
            }
            else
            {
                enemyNewCardIndex--;
                cards = enemyCards;
                yPosition = 1;
            }

            if (cards.Count >= 3)
                return;

            Vector2 emptyPosition = GameData.centralSectionOrigin + new Vector2(0, GameData.cardSpacing.y * yPosition);
            int i = 0;
            while (i < 3)
            {
                Vector2 tempValue = GameData.centralSectionOrigin + new Vector2(GameData.cardSpacing.x * i, GameData.cardSpacing.y * yPosition);
                
                if (i >= cards.Count)
                {
                    emptyPosition = tempValue;
                    break;
                }


                Card c = cards[i];
                if (Mathf.Abs(c.transform.localPosition.x - tempValue.x) > 0.1f)
                {
                    emptyPosition = tempValue;
                    break;
                }
                i++;
            }

            cardGlow.gameObject.SetActive(false);

            if (i < cards.Count)
                cards.Insert(i, card);
            else
                cards.Add(card);

            card.gameObject.SetActive(true);

            card.transform.parent = middleSectionParent;
            card.transform.localScale = new Vector3(1, 1, 1);

            card.transform.localPosition = emptyPosition;

            card.isInDeck = false;

            if (card.hasSpecialEffect)
            {
                if(playerMove)
                {
                    playerHealth += 5;
                    playerHealthTMPro.text = playerHealth.ToString();
                }
                else
                {
                    enemyHealth += 5;
                    enemyHealthTMPro.text = enemyHealth.ToString();
                }
            }
        }












        //---test functions
        public GameObject endgamePanel;
        public TextMeshProUGUI endGameText;
        public GameObject eventHandler;
        public void EndGamePanel()
        {
            endgamePanel.SetActive(true);
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
