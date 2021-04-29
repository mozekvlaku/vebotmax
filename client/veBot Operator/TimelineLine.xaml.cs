using System.Windows.Controls;

namespace veBot_Operator
{
    /// <summary>
    /// Interaction logic for TimelineMark.xaml
    /// </summary>
    public partial class TimelineLine : UserControl
    {
        Timeline parent;
    

        /// <summary>
        /// Creates the visual TimelineElement
        /// </summary>
        /// <param name="height">Height of the element (typically height of the Timeline's inner 'border' field)</param>
        /// <param name="seconds">Position on the timeline in seconds</param>
        public TimelineLine(Timeline parent)
        {
            InitializeComponent();

         
        }

     
        // Called by the parent to give it updated seconds based on its position
        public void SetSeconds(int seconds, int height)
        {
            rectOuter.Height = height - 2;
        }

       
    }
}
