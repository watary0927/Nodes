using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public InputField inputField;
    void Start()
	{
        inputField = inputField.GetComponent<InputField>();
        if (!PlayerPrefs.HasKey("Name")) PlayerPrefs.SetString("Name", "Noname");
        ShowInputField();
    }

    public void InputLogger()
    {
        string inputValue = inputField.text;
        Debug.Log(inputValue);
        PlayerPrefs.SetString("Name", inputValue);
        ShowInputField();
    }



    /// <summary>
    /// InputFieldの初期化用メソッド
    /// 入力値をリセットして、フィールドにフォーカスする
    /// </summary>


    void ShowInputField()
    {
        // 値をリセット
        inputField.text= PlayerPrefs.GetString("Name");

    }

}