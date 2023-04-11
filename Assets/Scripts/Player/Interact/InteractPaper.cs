using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractPaper : MonoBehaviour
{
    [Header("�����")]
    [SerializeField] Image text;
    [Header("����")]
    [SerializeField] AudioSource clip;
    public void Read(bool status)
    {
        text.gameObject.SetActive(status);
        Time.timeScale = Convert.ToInt16(!status);
        clip.Play();
            }
}
