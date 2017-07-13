using UnityEngine;
using UnityEngine.UI;

public class MVRInputUI : MonoBehaviour {

    public Button button;
    public Text inputText;
	 
	// Update is called once per frame
	public void OnSubmit () {
        
        if (MVRController.instance.Connect(inputText.text) == MVRInput.MVRInputStatus.CONNECTED)
        {
            Destroy(button.gameObject);
            Destroy(inputText.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
	}
}
