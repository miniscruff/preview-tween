namespace PreviewTween.Samples
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public sealed class ButtonLoadScene : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(_sceneName);
            });
        }
    }
}