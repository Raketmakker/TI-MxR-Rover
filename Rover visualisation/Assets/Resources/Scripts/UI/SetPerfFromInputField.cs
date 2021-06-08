using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPerfFromInputField : MonoBehaviour
{
    public Config.PerfKeys key;
    public InputField text;
    public enum DataType { @float, @int }
    public DataType type;
    public float minimum = 1;
    public float maximum = 100;
    public string defaultValue;

    private void Awake()
    {
        switch (this.type)
        {
            case DataType.@float:
                if (PlayerPrefs.HasKey(this.key.ToString()))
                {
                    this.text.text = PlayerPrefs.GetFloat(this.key.ToString()) + "";
                }
                else
                {
                    float fValue;
                    if (float.TryParse(this.defaultValue, out fValue))
                    {
                        this.text.text = this.defaultValue;
                        PlayerPrefs.SetFloat(this.key.ToString(), Mathf.Clamp(fValue, this.minimum, this.maximum));
                        PlayerPrefs.Save();
#if UNITY_EDITOR
                        Debug.Log("Saved float");
#endif
                    }
                }
                break;
            case DataType.@int:
                if (PlayerPrefs.HasKey(this.key.ToString()))
                {
                    this.text.text = PlayerPrefs.GetInt(this.key.ToString()) + "";
                }
                else
                {
                    int iValue;
                    if (int.TryParse(this.defaultValue, out iValue))
                    {
                        this.text.text = this.defaultValue;
                        PlayerPrefs.GetInt(this.key.ToString(), (int)Mathf.Clamp(iValue, this.minimum, this.maximum));
                        PlayerPrefs.Save();
#if UNITY_EDITOR
                        Debug.Log("Saved int");
#endif
                    }
                }
                break;
        }
    }

    public void OnDoneEditing()
    {
        switch (this.type)
        {
            case DataType.@float:
                float floatValue;
                if (float.TryParse(this.text.text, out floatValue))
                {
                    PlayerPrefs.SetFloat(this.key.ToString(), Mathf.Clamp(floatValue, this.minimum, this.maximum));
                    PlayerPrefs.Save();
#if UNITY_EDITOR
                    Debug.Log("Saved float");
#endif
                }
                break;
            case DataType.@int:
                int intValue;
                if (int.TryParse(this.text.text, out intValue))
                {
                    PlayerPrefs.SetInt(this.key.ToString(), (int)Mathf.Clamp(intValue, this.minimum, this.maximum));
                    PlayerPrefs.Save();
#if UNITY_EDITOR
                    Debug.Log("Saved int");
#endif
                }
                break;
        }
    }
}
