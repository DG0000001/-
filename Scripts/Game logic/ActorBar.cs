using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ActorBar : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject Rote;
    private Vector3 initialPosition;
    private bool isDragging;
    private Vector3 olderScale;
    private Color olderColor;
    public GameObject BG;
    public GameObject ActorState;
    private RoteData roteData;

    public Text countdownText;
    public Slider countdownSlider;

    private float countdownCurrentTime = 0;
    private void Start()
    {
        roteData = DataManage.Instance.GetRoteData(Rote.name);
        initialPosition = transform.position;
        isDragging = false;


    }
    private void Update()
    {
        if (countdownCurrentTime > 0)
        {
            countdownCurrentTime -= Time.deltaTime;
            countdownText.text = ((int)countdownCurrentTime).ToString();
            countdownSlider.value = countdownCurrentTime / roteData.CD;
        }
        else
        {
            SetCoolDownEnable(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BG.SetActive(true);
        Time.timeScale = 0.25f;
        ActorState.SetActive(true);
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);

        ActorState.transform.Find("Name").GetComponent<Text>().text = roteData.name;
        ActorState.transform.Find("HP").GetComponent<TextMeshProUGUI>().text = roteData.HP.ToString();
        ActorState.transform.Find("ATK").GetComponent<TextMeshProUGUI>().text = roteData.ATK.ToString();
        ActorState.transform.Find("DEF").GetComponent<TextMeshProUGUI>().text = roteData.DEF.ToString();
        ActorState.transform.Find("ATS").GetComponent<TextMeshProUGUI>().text = roteData.ATS.ToString();
        ActorState.transform.Find("Occupation").GetComponent<TextMeshProUGUI>().text = roteData.Occupation;
        ActorState.transform.Find("RagePoint").GetComponent<TextMeshProUGUI>().text = roteData.RagePoint.ToString();
        ActorState.transform.Find("PurchasePoint").GetComponent<TextMeshProUGUI>().text = roteData.PurchasePoint.ToString();
        ActorState.transform.Find("AttackRange").GetComponent<TextMeshProUGUI>().text = roteData.AttackRangeKey.ToString();

        if (GamePageManage.Instance.DetectionPurchasePoint(roteData))
        {
            isDragging = true;
            olderScale = transform.GetComponent<RectTransform>().localScale;
            olderColor = transform.GetComponent<Image>().color;
            transform.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f);
            transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GamePageManage.Instance.DetectionPurchasePoint(roteData))
        {
            isDragging = false;
            transform.GetComponent<RectTransform>().localScale = olderScale;
            transform.GetComponent<Image>().color = olderColor;
            transform.position = initialPosition;

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置创建一条射线
            Debug.DrawRay(worldPosition, ray.direction * 1200f, Color.red, 100f); // 在场景视图中绘制射线（仅用于调试

            RaycastHit[] hits = Physics.RaycastAll(ray, 1200f);
            if (hits.Length > 0) // 发射射线并检查是否击中了物体
            {
                foreach (RaycastHit hit in hits)
                {
                    GameObject hitObject = hit.collider.gameObject; // 获取被击中的物体
                    if (hitObject.CompareTag("StandHighPlatform") || hitObject.CompareTag("StandGroundPlatform"))
                    {
                        // 在这里进行处理，比如打印物体名称
                        Debug.Log("射中物体：" + hitObject.name);
                        //初始化角色
                        GameObject newRote = Instantiate(Rote);
                        newRote.name = Rote.name;
                        newRote.transform.position = hitObject.transform.position + Vector3.forward * 30;
                        newRote.GetComponent<Rote>().actorBar = this;
                        transform.parent.gameObject.SetActive(false);
                        ActorState.SetActive(false);
                        BG.GetComponent<Image>().raycastTarget = false;

                        break;
                    }
                }
            }
        }
    }
    public void SetCoolDownEnable(bool enable)
    {
        if (enable)
        {
            countdownCurrentTime = roteData.CD;
            countdownSlider.gameObject.SetActive(true);
            gameObject.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
            gameObject.GetComponent<Image>().raycastTarget = false;
        }
        else
        {
            countdownSlider.gameObject.SetActive(false);
            gameObject.GetComponent<Image>().color = Color.white;
            gameObject.GetComponent<Image>().raycastTarget = true;
            countdownSlider.value = 1;
        }
    }


}
