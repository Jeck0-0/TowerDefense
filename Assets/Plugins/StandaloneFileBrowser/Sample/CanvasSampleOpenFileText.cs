using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.Networking;


namespace TowerDefense
{
    [RequireComponent(typeof(Button))]
    public class CanvasSampleOpenFileText : MonoBehaviour, IPointerDownHandler {
        public Text output;
    
    #if UNITY_WEBGL && !UNITY_EDITOR
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
    
        public void OnPointerDown(PointerEventData eventData) {
            UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
        }
    
        // Called from browser
        public void OnFileUpload(string url) {
            StartCoroutine(OutputRoutine(url));
        }
    #else
        //
        // Standalone platforms & editor
        //
        public void OnPointerDown(PointerEventData eventData) { }
    
        void Start() {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }
    
        private void OnClick() {
            var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
            if (paths.Length > 0) {
                StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
            }
        }
    #endif
    
        private IEnumerator OutputRoutine(string url) {
            var loader = UnityWebRequest.Get(url);
            var async = loader.SendWebRequest();
            yield return new WaitUntil(() => async.isDone);
            output.text = async.webRequest.downloadHandler.text;
        }
    }
}
