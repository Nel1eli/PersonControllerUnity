using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractPaper : MonoBehaviour
{
    [Header("Текст")]
    [SerializeField] Image text;
    [Header("Звук")]
    [SerializeField] AudioSource clip;
    public void Read(bool status)
    {
        text.gameObject.SetActive(status);
        Time.timeScale = Convert.ToInt16(!status);
        clip.Play();
            }
}
