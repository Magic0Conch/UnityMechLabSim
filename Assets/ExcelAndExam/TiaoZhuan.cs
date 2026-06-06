using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiaoZhuan : MonoBehaviour
{
    StartExam exam;
    int Bname;
    Color Red;
    Color Green;
    private void Update()
    {
        print("btn en");
        gameObject.GetComponent<Image>().color = new Color((221 / 255f), (221 / 255f), (221 / 255f), (221 / 255f));
        if (exam == null )
            return;
        if (exam.examTimu.Tixing[Bname] == "选择" && exam.examTimu.YourAns[Bname] != "4"
            || exam.examTimu.Tixing[Bname] == "填空" && exam.examTimu.YourAns[Bname] != "")//回答过此题
        {
            Debug.Log("i  dsd");
            //this.GetComponent<Image>().color = Color.blue;
            gameObject.GetComponent<Image>().color = Color.gray;
        }
        if (exam.endF == true && exam.examTimu.YourAns[Bname] != exam.examTimu.TrueA[Bname])
            gameObject.GetComponent<Image>().color = Red;
        else if (exam.endF == true && exam.examTimu.YourAns[Bname] == exam.examTimu.TrueA[Bname])
            gameObject.GetComponent<Image>().color = Green;
    }
    // Start is called before the first frame update
    void Start()
    {
        exam = GameObject.Find("ExamPanel").GetComponent<StartExam>();
        Bname = int.Parse(gameObject.name.ToString());

        Red = new Color((250 / 255f), (29 / 255f), (27 / 255f), (255 / 255f));
        Green = new Color((124 / 255f), (229 / 255f), (120 / 255f), (255 / 255f));
    }

    // Update is called once per frame


    public void TiaoZhuanToTiMu()
    {
        //OnEnable();
        exam.n = Bname;
        exam.TiMu(Bname);
        if (Bname == 1)
        {
            //exam.EndB.SetActive(false);
            exam.LastB.SetActive(false);
            exam.NextB.SetActive(true);
        }
        else if (Bname == exam.examTimu.NUM)
        {
            exam.NextB.SetActive(false);
            exam.LastB.SetActive(true);
            //exam.EndB.SetActive(true);
        }
        else
        {
            //exam.EndB.SetActive(false);
            exam.LastB.SetActive(true);
            exam.NextB.SetActive(true);
        }
    }
}
