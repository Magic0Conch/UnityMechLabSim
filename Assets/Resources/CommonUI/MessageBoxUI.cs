using UnityEngine;
using UnityEngine.UI;

public delegate void OnCancel();
public delegate void OnSure();

public enum OpenMessageType
{
    Sure,
    SureandCancle
}

public class DialogInfo
{
    public string warnInfo;
    public string sureBtnInfo = "确定";
    public string cancleBtnInfo = "取消";
    public OnCancel onCancel;
    public OnSure onSure;
    public OpenMessageType openType;
}

public class MessageBoxUI
{
    private GameObject messageBox = null;

    private Button m_sureBtn;
    private Button m_cancelBtn;
    private Text m_infoTxt;
    private Text sureBtnTxt;
    private Text cancleBtnTxt;

    private DialogInfo m_dialogInfo;

    public MessageBoxUI(object val)
    {
        messageBox = GameObject.Instantiate(Resources.Load("CommonUI/MessageBox")) as GameObject;
        messageBox.transform.parent = GameObject.Find("MessageBox").transform;
        messageBox.transform.localPosition = Vector3.zero;
        messageBox.transform.localScale = Vector3.one;

        m_sureBtn = messageBox.transform.Find("BtnGroup/SureBtn").GetComponent<Button>();
        m_cancelBtn = messageBox.transform.Find("BtnGroup/CancleBtn").GetComponent<Button>();
        m_infoTxt = messageBox.transform.Find("Info").GetComponent<Text>();
        sureBtnTxt = messageBox.transform.Find("BtnGroup/SureBtn/Text").GetComponent<Text>();
        cancleBtnTxt = messageBox.transform.Find("BtnGroup/CancleBtn/Text").GetComponent<Text>();

        m_dialogInfo = (DialogInfo)val;
        if (m_dialogInfo.openType == OpenMessageType.Sure)
        {
            m_cancelBtn.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(m_dialogInfo.sureBtnInfo))
        {
            sureBtnTxt.text = m_dialogInfo.sureBtnInfo;
        }
        if (!string.IsNullOrEmpty(m_dialogInfo.cancleBtnInfo))
        {
            cancleBtnTxt.text = m_dialogInfo.cancleBtnInfo;
        }
        m_infoTxt.text = m_dialogInfo.warnInfo;

        m_sureBtn.onClick.AddListener(OnSureClick);
        m_cancelBtn.onClick.AddListener(OnCancelClick);
    }

    private void OnSureClick()
    {
        if (m_dialogInfo.onSure != null)
            m_dialogInfo.onSure();
        ClosePanel();
    }

    private void OnCancelClick()
    {
        if (m_dialogInfo.onCancel != null)
            m_dialogInfo.onCancel();
        ClosePanel();
    }

    private void ClosePanel()
    {
        GameObject.Destroy(messageBox);
    }
}
