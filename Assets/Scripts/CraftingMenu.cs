using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingMenu : MonoBehaviour
{
    [SerializeField] private GameObject CMenu;
    private int screenWidth;
    private int screenHeight;

    private bool showCraftingMenu;

    void ScreenInit()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
   
    }

    void InitializeVars()
    {
        showCraftingMenu = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScreenInit();
        InitializeVars();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();

        if (!showCraftingMenu )
        {
            CMenu.gameObject.SetActive(false);
        }
        else if ( showCraftingMenu )
        {
            CMenu.gameObject.SetActive(true);
        }
    }

    void HandleInputs()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            ScreenInit();
        }
        if (Keyboard.current.cKey.isPressed)
        {
            showCraftingMenu = !showCraftingMenu;
        }
    }
}
