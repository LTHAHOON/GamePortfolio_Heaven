using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonClickCommand
{
    void Execute(GameObject window);
    ButtonIdentifier GetButtonIdentifier();
}

public class CloseButtonClickCommand : IButtonClickCommand
{
    private ButtonSystem _buttonSystem;

    public CloseButtonClickCommand(ButtonSystem buttonSystem)
    {
        _buttonSystem = buttonSystem;
    }

    public void Execute(GameObject window)
    {
        _buttonSystem.OnClickCloseButton(window);
    }

    public ButtonIdentifier GetButtonIdentifier()
    {
        return ButtonIdentifier.Close;
    }
}

public class OpenButtonClickCommand : IButtonClickCommand
{
    private ButtonSystem _buttonSystem;

    public OpenButtonClickCommand(ButtonSystem buttonSystem)
    {
        _buttonSystem = buttonSystem;
    }

    public void Execute(GameObject window)
    {
        _buttonSystem.OnClickOpenButton(window);
    }

    public ButtonIdentifier GetButtonIdentifier()
    {
        return ButtonIdentifier.Open;
    }
}

public enum ButtonIdentifier
{
    Close,
    Open
}

public class ButtonInvoker
{
    IButtonClickCommand[] _buttonClickCommand = new IButtonClickCommand[5];

    public void AddButtonClickCommand(IButtonClickCommand buttonClickCommand)
    {
        for (int i = 0; i < _buttonClickCommand.Length; i++)
        {
            if (_buttonClickCommand[i] == null)
            {
                _buttonClickCommand[i] = buttonClickCommand;
                return;
            }
        }
    }

    public void PressButton(ButtonIdentifier buttonIdentifer, GameObject window)
    {
        for (int i = 0; i < _buttonClickCommand.Length; i++)
        {
            ButtonIdentifier otherButtonIdentifier = _buttonClickCommand[i].GetButtonIdentifier();
            if (buttonIdentifer == otherButtonIdentifier)
            {
                _buttonClickCommand[i].Execute(window);
                return;
            }
        }
        Debug.Log("Not Existed buttonClickCommand");
    }
}


public class ButtonSystem : MonoBehaviour
{
    public static ButtonInvoker buttonInvoker;

    public void OnClickCloseButton(GameObject window) 
    {
        window.SetActive(false);
    }

    public void OnClickOpenButton(GameObject window) 
    {
        window.SetActive(true);
    }

    private void Awake()
    {
        IButtonClickCommand closeButtonClickCommand = new CloseButtonClickCommand(this);
        IButtonClickCommand openButtonClickCommand = new OpenButtonClickCommand(this);

        buttonInvoker = new ButtonInvoker();
        buttonInvoker.AddButtonClickCommand(closeButtonClickCommand);
        buttonInvoker.AddButtonClickCommand(openButtonClickCommand);
    }
}
