using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Capture : MonoBehaviour
{

    public Button screenShotButton;          // ��ü ȭ�� ĸ��
    public Button readAndShowButton; // ����� ��ο��� ��ũ���� ���� �о�ͼ� �̹����� ����
    public Image imageToShow;        // ��� �̹��� ������Ʈ

    //public ScreenShotFlash flash;

    public string folderName = "ScreenShots";
    public string fileName = "MyScreenShot";
    public string extName = "png";

    private bool _willTakeScreenShot = false;

    private Texture2D _imageTexture; // imageToShow�� �ҽ� �ؽ���
    private string RootPath { get { return Application.dataPath; } }
    private string FolderPath => $"{RootPath}/{folderName}";
    private string TotalPath => $"{FolderPath}/{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}.{extName}";

    private string lastSavedPath;


    private void Awake()
    {
        screenShotButton.onClick.AddListener(TakeScreenShotFull);
        readAndShowButton.onClick.AddListener(ReadScreenShotAndShow);
    }

    private void TakeScreenShotFull()
    {
        StartCoroutine(TakeScreenShotRoutine());
    }

    private void ReadScreenShotAndShow()
    {
        ReadScreenShotFileAndShow(imageToShow);
    }

    // UI ���� ���� ȭ�鿡 ���̴� ��� �� ĸ��
    private IEnumerator TakeScreenShotRoutine()
    {
        yield return new WaitForEndOfFrame();
        CaptureScreenAndSave();
    }

    private void CaptureScreenAndSave()
    {
        string totalPath = TotalPath; // ������Ƽ ���� �� �ð��� ���� �̸��� �����ǹǷ� ĳ��

        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);

        // ���� ��ũ�����κ��� ���� ������ �ȼ����� �ؽ��Ŀ� ����
        screenTex.ReadPixels(area, 0, 0);

        bool succeeded = true;
        try
        {
            // ������ �������� ������ ���� ����
            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            // ��ũ���� ����
            File.WriteAllBytes(totalPath, screenTex.EncodeToPNG());
        }
        catch (Exception e)
        {
            succeeded = false;
            Debug.LogWarning($"Screen Shot Save Failed : {totalPath}");
            Debug.LogWarning(e);
        }

        // ������ �۾�
        Destroy(screenTex);

        if (succeeded)
        {
            Debug.Log($"Screen Shot Saved : {totalPath}");
            //flash.Show(); // ȭ�� ��½
            lastSavedPath = totalPath; // �ֱ� ��ο� ����
        }
    }

    // ���� �ֱٿ� ����� �̹��� �����ֱ�
    private void ReadScreenShotFileAndShow(Image destination)
    {
        string folderPath = FolderPath;
        string totalPath = lastSavedPath;

        if (Directory.Exists(folderPath) == false)
        {
            Debug.LogWarning($"{folderPath} ������ �������� �ʽ��ϴ�.");
            return;
        }
        if (File.Exists(totalPath) == false)
        {
            Debug.LogWarning($"{totalPath} ������ �������� �ʽ��ϴ�.");
            return;
        }

        // ������ �ؽ��� �ҽ� ����
        if (_imageTexture != null)
            Destroy(_imageTexture);
        if (destination.sprite != null)
        {
            Destroy(destination.sprite);
            destination.sprite = null;
        }

        // ����� ��ũ���� ���� ��ηκ��� �о����
        try
        {
            byte[] texBuffer = File.ReadAllBytes(totalPath);

            _imageTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            _imageTexture.LoadImage(texBuffer);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"��ũ���� ������ �д� �� �����Ͽ����ϴ�.");
            Debug.LogWarning(e);
            return;
        }

        // �̹��� ��������Ʈ�� ����
        Rect rect = new Rect(0, 0, _imageTexture.width, _imageTexture.height);
        Sprite sprite = Sprite.Create(_imageTexture, rect, Vector2.one * 0.5f);
        destination.sprite = sprite;
    }
}