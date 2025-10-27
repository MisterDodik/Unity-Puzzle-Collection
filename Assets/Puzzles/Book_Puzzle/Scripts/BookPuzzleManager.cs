using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.puzzles.Book_Puzzle
{
    public class BookPuzzleManager : MonoBehaviour
    {
        [SerializeField] private Transform puzzleParent;

        [SerializeField] private BookPuzzleData puzzleData;

        [SerializeField] private int coinCount = 5;
        [SerializeField] private float moveTime = .5f;

        private List<CoinScript> spawnedCoins = new();
        private List<GameObject> spawnedGlowIndicators = new();

        //private List<(CoinScript? coin, Vector2 position)> coinSlots = new();
        private List<(CoinScript coin, Vector2 position)> coinSlots = new();

        private int progress = 0;
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
            //Disable mouse events
            eventSystem.SetActive(false);

            Sequence sequence = DOTween.Sequence();

            foreach (CoinScript coin in spawnedCoins)
            {
                var match = coinSlots.FirstOrDefault(i => i.coin != null && i.coin.startIndex == coin.targetIndex);
                if (match.coin != null)
                {
                    Vector2 finalPosition = match.coin.startPosition;
                    sequence.Join(coin.transform.DOLocalMove(finalPosition, moveTime).OnComplete(() =>{
                        coin.coinIndex = coin.targetIndex;
                    }));
                }
            }

            sequence.Play().OnComplete(() =>
            {
                foreach (CoinScript coin in spawnedCoins)
                    coin.CheckPosition();

                // Reenable mouse events
                eventSystem.SetActive(true);
            });
        }

        public void Completed()
        {
            print("Win");
        }
        private void OnCompleteEvents()
        {
            EndgamePanel();
            Completed();
        }
        private void InitGame()
        {
            progress = 0;

            for (int i = 0; i < coinCount; i++)
            {
                GameObject glow;

                if (i < spawnedGlowIndicators.Count)
                {
                    glow = spawnedGlowIndicators[i];
                }
                else
                {
                    var prefab = puzzleData.glowIndicatorData[i].glowIndicatorPrefab;
                    glow = Instantiate(prefab, puzzleParent);
                    spawnedGlowIndicators.Add(glow);
                }

                Vector3 position = puzzleData.glowIndicatorData[i].glowPosition;
                glow.gameObject.SetActive(false);
            }

            if (coinSlots.Count == 0)
            {
                for (int i = 0; i < coinCount + 1; i++)
                {
                    Vector2 pos = puzzleData.coinData[i].coinPosition;
                    coinSlots.Add((null, pos)); 
                }
            }

            for (int i = 0; i < coinCount + 1; i++)
            {
                if (i == coinCount) 
                {
                    coinSlots[i] = (null, coinSlots[i].position);
                    continue;
                }

                CoinScript coin;

                if (i < spawnedCoins.Count)
                {
                    coin = spawnedCoins[i];
                    coin.gameObject.SetActive(true);
                }
                else
                {
                    var prefab = puzzleData.coinData[i].coinPrefab;
                    coin = Instantiate(prefab, puzzleParent).GetComponent<CoinScript>();
                    spawnedCoins.Add(coin);
                }

                Vector3 position = coinSlots[i].position;
                coin.InitCoin(this, i, puzzleData.coinData[i].targetIndex, position);
                coinSlots[i] = (coin, position);
            }

        }

        public void TriggerGlow(int coinIndex, bool isCorrect, bool wasCorrect)
        {
            spawnedGlowIndicators[coinIndex].SetActive(isCorrect);

            if (isCorrect && !wasCorrect)
                progress++;
            else if (!isCorrect && wasCorrect)
                progress--;

            if (progress >= coinCount)
                OnCompleteEvents();
            else if (progress < 0)
                progress = 0;

        }


        public void MoveCoin(CoinScript coin)
        {
            Vector2 endPosition = Vector2.zero;
            int? selectedIndex = null;

            if (coin.coinIndex == coinCount) // Middle slot
            {
                for (int i = 0; i < coinSlots.Count; i++)
                {
                    if (i != coinCount && coinSlots[i].coin == null)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                int[] preferred = {
                    (coin.coinIndex + 2) % coinCount,
                    (coin.coinIndex + 3) % coinCount,
                    coinCount 
                };

                foreach (int i in preferred)
                {
                    if (coinSlots[i].coin == null)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            if (selectedIndex == null)
            {
                //Disable mouse events
                eventSystem.SetActive(false);

                coin.transform.DOShakePosition(moveTime, strength: new Vector3(0.2f, 0, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true).OnComplete(() =>
                {
                    //Enable mouse events
                    eventSystem.SetActive(true);

                    return;
                });

                return;
            }


            int newIndex = selectedIndex.Value;
            endPosition = coinSlots[newIndex].position;


            //Disable mouse events
            eventSystem.SetActive(false);

            coin.transform.DOLocalMove(endPosition, moveTime).OnComplete(() =>
            {
                coinSlots[coin.coinIndex] = (null, coinSlots[coin.coinIndex].position);
                coin.coinIndex = newIndex;
                coinSlots[newIndex] = (coin, endPosition);

                coin.CheckPosition();

                //Enable mouse events
                eventSystem.SetActive(true);
            });

        }









        //------test functions-----
        public GameObject eventSystem;
        public GameObject endPanel;
        private void EndgamePanel()
        {
            endPanel.SetActive(true);
        }
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
