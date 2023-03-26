using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Capture : MonoBehaviour
{

    public Button screenShotButton;          // 전체 화면 캡쳐
    public Button readAndShowButton; // 저장된 경로에서 스크린샷 파일 읽어와서 이미지에 띄우기
    public Image imageToShow;        // 띄울 이미지 컴포넌트

    //public ScreenShotFlash flash;

    public string folderName = "ScreenShots";
    public string fileName = "MyScreenShot";
    public string extName = "png";

    private bool _willTakeScreenShot = false;

    private Texture2D _imageTexture; // imageToShow의 소스 텍스쳐
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

    // UI 포함 현재 화면에 보이는 모든 것 캡쳐
    private IEnumerator TakeScreenShotRoutine()
    {
        yield return new WaitForEndOfFrame();
        CaptureScreenAndSave();
    }

    private void CaptureScreenAndSave()
    {
        string totalPath = TotalPath; // 프로퍼티 참조 시 시간에 따라 이름이 결정되므로 캐싱

        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);

        // 현재 스크린으로부터 지정 영역의 픽셀들을 텍스쳐에 저장
        screenTex.ReadPixels(area, 0, 0);

        bool succeeded = true;
        try
        {
            // 폴더가 존재하지 않으면 새로 생성
            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            // 스크린샷 저장
            File.WriteAllBytes(totalPath, screenTex.EncodeToPNG());
        }
        catch (Exception e)
        {
            succeeded = false;
            Debug.LogWarning($"Screen Shot Save Failed : {totalPath}");
            Debug.LogWarning(e);
        }

        // 마무리 작업
        Destroy(screenTex);

        if (succeeded)
        {
            Debug.Log($"Screen Shot Saved : {totalPath}");
            //flash.Show(); // 화면 번쩍
            lastSavedPath = totalPath; // 최근 경로에 저장
        }
    }

    // 가장 최근에 저장된 이미지 보여주기
    private void ReadScreenShotFileAndShow(Image destination)
    {
        string folderPath = FolderPath;
        string totalPath = lastSavedPath;

        if (Directory.Exists(folderPath) == false)
        {
            Debug.LogWarning($"{folderPath} 폴더가 존재하지 않습니다.");
            return;
        }
        if (File.Exists(totalPath) == false)
        {
            Debug.LogWarning($"{totalPath} 파일이 존재하지 않습니다.");
            return;
        }

        // 기존의 텍스쳐 소스 제거
        if (_imageTexture != null)
            Destroy(_imageTexture);
        if (destination.sprite != null)
        {
            Destroy(destination.sprite);
            destination.sprite = null;
        }

        // 저장된 스크린샷 파일 경로로부터 읽어오기
        try
        {
            byte[] texBuffer = File.ReadAllBytes(totalPath);

            _imageTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            _imageTexture.LoadImage(texBuffer);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"스크린샷 파일을 읽는 데 실패하였습니다.");
            Debug.LogWarning(e);
            return;
        }

        // 이미지 스프라이트에 적용
        Rect rect = new Rect(0, 0, _imageTexture.width, _imageTexture.height);
        Sprite sprite = Sprite.Create(_imageTexture, rect, Vector2.one * 0.5f);
        destination.sprite = sprite;
    }
}