using UnityEngine;

public class NPCCarrier : Carrier
{
    public Carrier targetCarrier;
    public new bool enabled = true;
    public MoneyPile moneyPile;
    [SerializeField] Animator myAnimator;
    public int waitTime = 2;
    private Item CurrentItem { get; set; }

    private float _timer;
    private bool _isProcessingBox;
    private GameManager _gameManager;
    private Bank _bank;
    private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

    private void Start()
    {
        // cache instances
        _gameManager = GameManager.Instance;
        _bank = Bank.Instance;
        
    }

    private void Update()
    {
        if (targetCarrier && enabled && targetCarrier.CheckCanGiveBoxes() && CheckCanReceiveBoxes() && !_isProcessingBox)
        {
            // Take the box from the targetCarrier
            Debug.Log("Taking box from targetCarrier");
            myAnimator.SetFloat(ForwardSpeed, 100f);
            ReceiveBox(targetCarrier.GiveBox());

            _isProcessingBox = true;
            _timer = waitTime;
        }
        else if (_isProcessingBox)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Debug.Log("Giving box to targetCarrier");
                myAnimator.SetFloat(ForwardSpeed, 0);

                GiveBox();
                CurrentItem = _gameManager.CurrentLevel.GetRandomItemForLevel();
                _gameManager.UnlockItem(CurrentItem);
                Debug.Log("Got new item: " + CurrentItem.name);
                _bank.AddMoneyToPile(moneyPile);

                _isProcessingBox = false;
            }
        }
    }
}