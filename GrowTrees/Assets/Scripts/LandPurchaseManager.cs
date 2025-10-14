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

    [Header("���������")]
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
            Debug.Log("������ ������� ���������");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelPurchase);
            Debug.Log("������ ������ ���������");
        }
    }

    public void ShowPurchasePanel(LandSection section)
    {
        if (isPanelActive)
        {
            Debug.Log("������ ��� �������, ���������� �����");
            return;
        }

        Debug.Log($"���������� ������ ������� ��� ������: {section.sectionName}");

        currentSection = section;

        if (sectionNameText != null)
            sectionNameText.text = section.sectionName;
        if (costText != null)
            costText.text = $"���������: {section.unlockCost} �����";

        if (purchaseButton != null)
        {
            purchaseButton.interactable = false;
            Debug.Log("������ ������� �������� ���������");
        }
        if (cancelButton != null)
        {
            cancelButton.interactable = false;
            Debug.Log("������ ������ �������� ���������");
        }

        buttonsEnabled = false;

        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
            Debug.Log("������ ������� ������������");
        }

        isPanelActive = true;

        StartCoroutine(EnableButtonsAfterDelay());
    }

    IEnumerator EnableButtonsAfterDelay()
    {
        Debug.Log($"��������� �������� {clickDelay} ������");
        yield return new WaitForSeconds(clickDelay);

        bool canAfford = GameManager.Instance.coins >= currentSection.unlockCost;

        if (purchaseButton != null)
        {
            purchaseButton.interactable = canAfford;
            Debug.Log($"������ �������: {canAfford} (�����: {GameManager.Instance.coins}, �����: {currentSection.unlockCost})");
        }

        if (cancelButton != null)
        {
            cancelButton.interactable = true;
            Debug.Log("������ ������ ������������");
        }

        buttonsEnabled = true;
        Debug.Log("������ ������������ ����� ��������");
    }

    void PurchaseSection()
    {
        Debug.Log("������ ������ �������!");

        if (!buttonsEnabled)
        {
            Debug.Log("������ ��� �� ������������ - ���������� �������");
            return;
        }

        if (currentSection == null)
        {
            Debug.LogError("������� ������ �� �����������!");
            return;
        }

        Debug.Log($"�������� ������ ������: {currentSection.sectionName} �� {currentSection.unlockCost} �����");

        if (GameManager.Instance.SpendCoins(currentSection.unlockCost))
        {
            Debug.Log($"������� ������� ������: {currentSection.sectionName}");
            currentSection.Unlock();
            ClosePanel();
        }
        else
        {
            Debug.Log($"������������ �����! �����: {currentSection.unlockCost}, ����: {GameManager.Instance.coins}");
        }
    }

    void CancelPurchase()
    {
        Debug.Log("������ ������ ������");

        if (!buttonsEnabled)
        {
            Debug.Log("������ ��� �� ������������ - ���������� �������");
            return;
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        Debug.Log("��������� ������ �������");

        if (purchasePanel != null)
            purchasePanel.SetActive(false);

        currentSection = null;
        isPanelActive = false;
        buttonsEnabled = false;
    }

    public void OnCellClicked(MapCell cell)
    {
        Debug.Log($"���� �� ������: {cell.gridX}, {cell.gridY}, ��������������: {cell.isUnlocked}");

        if (cell.isUnlocked)
        {
            Debug.Log("������ ��� �������������� - ����������");
            return;
        }

        if (isPanelActive)
        {
            Debug.Log("������ ��� ������� - ����������");
            return;
        }

        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager.Instance �� ������!");
            return;
        }

        foreach (LandSection section in MapManager.Instance.landSections)
        {
            if (section.cells.Contains(cell.gameObject))
            {
                if (!section.isUnlocked)
                {
                    Debug.Log($"������� ��������������� ������: {section.sectionName}");
                    ShowPurchasePanel(section);
                }
                else
                {
                    Debug.Log($"������ {section.sectionName} ��� ��������������");
                }
                return;
            }
        }

        Debug.LogWarning($"�� ������� ������ ��� ������ {cell.gridX}, {cell.gridY}");
    }
}