using UnityEngine;
using UnityEngine.UIElements;

namespace MyUILibrary
{
    // Derives from BaseField<bool> base class. Represents a container for its input part.
    public class SlideToggle : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<SlideToggle, UxmlTraits> { }

        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription> { }

        // In the spirit of the BEM standard, the SlideToggle has its own block class and two element classes. It also
        // has a class that represents the enabled state of the toggle.
        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";

        VisualElement m_Input;
        VisualElement m_Knob;

        // Custom controls need a default constructor. This default constructor calls the other constructor in this
        // class.
        public SlideToggle() : this(null) { }

        // This constructor allows users to set the contents of the label.
        public SlideToggle(string label) : base(label, new VisualElement())
        {
            // Style the control overall.
            AddToClassList(ussClassName);

            // Get the BaseField's visual input element and use it as the background of the slide.
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClassName);
            m_Input.name = "Input";
            m_Input.style.unitySliceBottom = 1;
            m_Input.style.justifyContent = Justify.Center;
            m_Input.style.maxWidth = 30;
            m_Input.style.maxHeight = 16;
            m_Input.style.borderColor = Color.black;
            m_Input.style.borderBottomWidth = 1;
            m_Input.style.borderLeftWidth = 1;
            m_Input.style.borderRightWidth = 1;
            m_Input.style.borderTopWidth = 1;
            m_Input.style.borderBottomLeftRadius = 8;
            m_Input.style.borderBottomRightRadius = 8;
            m_Input.style.borderTopLeftRadius = 8;
            m_Input.style.borderTopRightRadius = 8;
            m_Input.style.marginBottom = 0;
            m_Input.style.marginLeft = 2;
            m_Input.style.marginRight = 2;
            m_Input.style.marginTop = 2;
            m_Input.AddManipulator(new MouseEventLogger());
            Add(m_Input);

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            m_Knob = new VisualElement();
            m_Knob.AddToClassList(inputKnobUssClassName);
            m_Knob.name = "Knob";
            m_Knob.style.backgroundImage = new StyleBackground((Texture2D)UnityEditor.EditorGUIUtility.Load("pre slider thumb@2x"));
            m_Knob.style.width = 17;
            m_Knob.style.height = 17;
            m_Knob.style.minHeight = 17;
            m_Knob.style.marginBottom = 1;
            m_Knob.style.top = new StyleLength(new Length(3,LengthUnit.Pixel));
            m_Input.Add(m_Knob);

            // There are three main ways to activate or deactivate the SlideToggle. All three event handlers use the
            // static function pattern described in the Custom control best practices.

            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));
        }

        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            if (slideToggle.panel?.contextType == ContextType.Player)
                return;

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        // All three callbacks call this method.
        void ToggleValue()
        {
            value = !value;
        }

        // Because ToggleValue() sets the value property, the BaseField class fires a ChangeEvent. This results in a
        // call to SetValueWithoutNotify(). This example uses it to style the toggle based on whether it's currently
        // enabled.
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            //This line of code styles the input element to look enabled or disabled.
            m_Input.EnableInClassList(inputCheckedUssClassName, newValue);
            
            if (base.value)
            {
            	m_Input.style.backgroundColor = Color.green;
            	m_Knob.style.left = new StyleLength(new Length(50,LengthUnit.Percent));
            }
            else
            {
            	m_Input.style.backgroundColor = Color.gray;
            	m_Knob.style.left = new StyleLength(new Length(0,LengthUnit.Percent));
            }
        }
    
	    class MouseEventLogger : Manipulator
	    {
	    	protected override void RegisterCallbacksOnTarget()
	    	{
	    		target.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
	    	}
	    	
	    	protected override void UnregisterCallbacksFromTarget()
	    	{
	    		target.UnregisterCallback<MouseDownEvent>(OnMouseDownEvent);
	    	}
	    	
	    	void OnMouseDownEvent(MouseEventBase<MouseDownEvent> evt)
	    	{
	    		var element = evt.currentTarget as VisualElement;
	    		var slideToggle = element.parent as SlideToggle;
	    		slideToggle.ToggleValue();
	    	}
	    }
    }
}
