using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Ink.Runtime;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class GradientDialogue : MonoBehaviour {
	int[] coordinates;
	bool first;

	void Awake () {
		coordinates = new int[2];
		coordinates[0] = 0;
		coordinates[1] = 0;

		first = true;
		// Remove the default message
		RemoveChildren();
		StartStory();
	}

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory () {
		story = new Story (inkJSONAsset.text);
		first = true;
		RefreshView();
	}
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Remove all the UI on screen
		RemoveChildren ();
		
		int index = 0;
		// Read all the content until we can't continue any more
		while (story.canContinue) {
			// Continue gets the next line of the story
			string text = story.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text, index);
			index++;
		}

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			for (int i = 0; i < story.currentChoices.Count; i++) {
				Choice choice = story.currentChoices [i];
				Button button = CreateChoiceView (choice.text.Trim (), i);
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					coordinates[0] = (int)button.transform.position.x; 
					coordinates[1] = (int)button.transform.position.y;
					print(coordinates);
					OnClickChoiceButton (choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			Button choice = CreateChoiceView("End of story.\nRestart?", 0);
			choice.onClick.AddListener(delegate{
				StartStory();
			});
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		
		RefreshView();
	}

	// Creates a button showing the choice text
	void CreateContentView (string text, int index) {
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.position = new Vector3(0, 200 - index * 100, 0);
		storyText.transform.SetParent (canvas.transform, false);
	}

	// Creates a button showing the choice text
	Button CreateChoiceView (string text, int index) {
		// Creates the button from a prefab
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (canvas.transform, false);
		choice.transform.position = new Vector3(422, 150, 0);
		
		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text> ();
		// choiceText.text = text;
		choiceText.text = "   ";

		int spc = 15;
		// Rendering by location
		if (first) {
			if (index == 0) {
				choice.transform.position += new Vector3(-spc, spc, 0);
			}
			if (index == 1) {
				choice.transform.position += new Vector3(spc, spc, 0);
			}
			if (index == 2) {
				choice.transform.position += new Vector3(-spc, -spc, 0);
			}
			if (index == 3) {
				first = false;
				choice.transform.position += new Vector3(spc, -spc, 0);
			}
		}
		else {
			if (index == 0) {
				choice.transform.position = new Vector3(coordinates[0], coordinates[1] + spc*2, 0);
			}
			if (index == 1) {
				choice.transform.position = new Vector3(coordinates[0] + spc*2, coordinates[1], 0);
			}
			if (index == 2) {
				choice.transform.position = new Vector3(coordinates[0], coordinates[1] - spc*2, 0);
			}
			if (index == 3) {
				choice.transform.position = new Vector3(coordinates[0] - spc*2, coordinates[1], 0);
			}
		}
		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			if (canvas.transform.GetChild(i).gameObject.GetComponent<BoxCollider>() == null) {
				GameObject.Destroy (canvas.transform.GetChild (i).gameObject);
			}
		}
	}

	[SerializeField]
	private TextAsset inkJSONAsset;
	private Story story;

	[SerializeField]
	private Canvas canvas;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab;
	[SerializeField]
	private Button buttonPrefab;
}