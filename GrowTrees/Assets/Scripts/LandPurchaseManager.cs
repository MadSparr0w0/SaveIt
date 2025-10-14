using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LandPurchaseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject purchasePanel;
    public Text sectionNameText;
    public Text costText;
    public Button purchaseButton;
    public Button cancelButton;

    [Header("Настройки")]
    public float clickDelay = 0.3f;

    private LandSection currentSection;
    private bool isPanelActive = false;
    private bool buttonsEnabled = false;

    public static LandPurchaseManager Instance;

    void Awake()
    {
        Instance = this;
        if (purchasePanel != null)
            purchasePanel.SetActive(false);

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(PurchaseSection);
            Debug.Log("Кнопка покупки настроена");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelPurchase);
            Debug.Log("Кнопка отмены настроена");
        }
    }

    public void ShowPurchasePanel(LandSection section)
    {
        if (isPanelActive)
        {
            Debug.Log("Панель уже активна, игнорируем вызов");
            return;
        }

        Debug.Log($"Показываем панель покупки для секции: {section.sectionName}");

        currentSection = section;

        if (sectionNameText != null)
            sectionNameText.text = section.sectionName;
        if (costText != null)
            costText.text = $"Стоимость: {section.unlockCost} монет";

        if (purchaseButton != null)
        {
            purchaseButton.interactable = false;
            Debug.Log("Кнопка покупки временно отключена");
        }
        if (cancelButton != null)
        {
            cancelButton.interactable = false;
            Debug.Log("Кнопка отмены временно отключена");
        }

        buttonsEnabled = false;

        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
            Debug.Log("Панель покупки активирована");
        }

        isPanelActive = true;

        StartCoroutine(EnableButtonsAfterDelay());
    }

    IEnumerator EnableButtonsAfterDelay()
    {
        Debug.Log($"Запускаем задержку {clickDelay} секунд");
        yield return new WaitForSeconds(clickDelay);

        bool canAfford = GameManager.Instance.coins >= currentSection.unlockCost;

        if (purchaseButton != null)
        {
            purchaseButton.interactable = canAfford;
            Debug.Log($"Кнопка покупки: {canAfford} (монет: {GameManager.Instance.coins}, нужно: {currentSection.unlockCost})");
        }

        if (cancelButton != null)
        {
            cancelButton.interactable = true;
            Debug.Log("Кнопка отмены активирована");
        }

        buttonsEnabled = true;
        Debug.Log("Кнопки активированы после задержки");
    }

    void PurchaseSection()
    {
        Debug.Log("Нажата кнопка покупки!");

        if (!buttonsEnabled)
        {
            Debug.Log("Кнопки еще не активированы - игнорируем нажатие");
            return;
        }

        if (currentSection == null)
        {
            Debug.LogError("Текущая секция не установлена!");
            return;
        }

        Debug.Log($"Пытаемся купить секцию: {currentSection.sectionName} за {currentSection.unlockCost} монет");

        if (GameManager.Instance.SpendCoins(currentSection.unlockCost))
        {
            Debug.Log($"Успешно куплена секция: {currentSection.sectionName}");
            currentSection.Unlock();
            ClosePanel();
        }
        else
        {
            Debug.Log($"Недостаточно монет! Нужно: {currentSection.unlockCost}, есть: {GameManager.Instance.coins}");
        }
    }

    void CancelPurchase()
    {
        Debug.Log("Нажата кнопка отмены");

        if (!buttonsEnabled)
        {
            Debug.Log("Кнопки еще не активированы - игнорируем нажатие");
            return;
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        Debug.Log("Закрываем панель покупки");

        if (purchasePanel != null)
            purchasePanel.SetActive(false);

        currentSection = null;
        isPanelActive = false;
        buttonsEnabled = false;
    }

    public void OnCellClicked(MapCell cell)
    {
        Debug.Log($"Клик по ячейке: {cell.gridX}, {cell.gridY}, разблокирована: {cell.isUnlocked}");

        if (cell.isUnlocked)
        {
            Debug.Log("Ячейка уже разблокирована - игнорируем");
            return;
        }

        if (isPanelActive)
        {
            Debug.Log("Панель уже активна - игнорируем");
            return;
        }

        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager.Instance не найден!");
            return;
        }

        foreach (LandSection section in MapManager.Instance.landSections)
        {
            if (section.cells.Contains(cell.gameObject))
            {
                if (!section.isUnlocked)
                {
                    Debug.Log($"Найдена заблокированная секция: {section.sectionName}");
                    ShowPurchasePanel(section);
                }
                else
                {
                    Debug.Log($"Секция {section.sectionName} уже разблокирована");
                }
                return;
            }
        }

        Debug.LogWarning($"Не найдена секция для ячейки {cell.gridX}, {cell.gridY}");
    }
}