   M S B A M L   `   `   `  ����I  EveBot Operator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:veBot_OperatorveBot_Operator  5   	   ` \System.Drawing.Primitives, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3aR?clr-namespace:System.Drawing;assembly=System.Drawing.PrimitivesSystem.Drawing 5   	   S Onpcook.Terminal.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulleHclr-namespace:npcook.Terminal.Controls;assembly=npcook.Terminal.Controlsnpcook.Terminal.Controls 6Y       veBot_Operator.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:veBot_Operator     5   	   SDrawing?clr-namespace:System.Drawing;assembly=System.Drawing.Primitives     5   	   YtermHclr-namespace:npcook.Terminal.Controls;assembly=npcook.Terminal.Controls     6Y      	MayaStyle#���   5	   	     � Title$  veBot Operator��5
   	   $	��600��6    $
��1000��60   $	��450��6@   $	��698��6M   .��5      ]� .+�5   
   o� $	��Top=�6   .@�5      m� $��_File��6   .@�5      m� $
��_New��6   m� 5      $��_Open��6   m� 5      $��_Save��6   �� 5      m� 5      $��_Exit��6   5      5      � 5   
    � RowDefinitions 5      �� 5      $��1* �6    �� 5      $��1* �6    .+�5      � $��0��6    � ColumnDefinitions 5      �� 5      $��1* �6'   �� 5      $��2* �6'   �� 5      $��1* �6'   .+�5       � $��0��6   .+�5"      �� $��
Connection��6   $��	10,5,10,0q�63   .��5#      �  5$      �� 5%   "   �� 5&   "    5(      �� 5)   "   $��20 �60   6>   �� 5*   "   $��18 �60   6>   �� 5+   "   $��30 �60   6>   �� 5,   "   $��30 �60   6>   �� 5-   "   $��30 �60   6>   �� 5.   "   $��18 �60   6>   �� 5/   "   $��30 �60   6>   �� 50   "   $��18 �60   6>   �� 51   "   $��30 �60   6>   .+�53      � $��0��6#   $��2��60   $��0,0,0,0q�6D    54   "   �� 55   &   $��90 �67   6D   �� 56   &   69   .+�58   "   �� $��Center=�6(   $��90��6C   $��14��6N   .��6Z   veBot IP Address6l   �� 59   "   -   5         ��Name$ comport��59   A   $��192.168.102.221��6*   $��1��6R   $
��Auto��6b   $��0,0,5,0q�6p   5:      �� 5;      $��1��6$   $��12(�61   $
��Bold"�6?   .��6Q   SSH connection6a   �� 5<      -   5        $��sshStatus_txt��5<   $   $��1��69   $��12(�6F   $��1��6T   $
��Bold"�6d   $��Center=�6v   $��Right=�6�   $��0,0,5,0q�6�   ���	#FFDE1B1B.��6�   •6�   �� 5=      -   5         ��Name$ 
sshEst_btn��5=   %   $��2��6R   $��0��6_   $��5,0,5,0q�6o   $��25��6�   .��6�   	Establish6�   �� 5>      -   5        $ sshClose_btn��5>   %   $��2��6V   $��1��6c   $��5,0,5,0q�6s   $��25��6�   .��6�   Close6�   �� 5?      -   5        $ sshStartScript_btn��5?   %   $��3��6b   $��0��6o   $��5,0,5,0q�6   $��25��6�   .��6�   Start script6�   �� 5@      -   5        $ sshStopScript_btn��5@   %   $��3��6`   $��1��6m   $��5,0,5,0q�6}   $��25��6�   .��6�   Stop script6�   �� 5A      -   5        $ sshResetScript_btn��5A   %   $��4��6b   $��2��6o   $��5,0,5,0q�6�   $��25��6�   .��6�   Reset script6�   �� 5B      $��5��6$   $��12(�61   $
��Bold"�6?   .��6Q   Camera controls6b   �� 5C      -   5        $��cameraStatus_txt��5C   $   $��5��6<   $��12(�6I   $��1��6W   $
��Bold"�6g   $��Center=�6y   $��Right=�6�   $��0,0,5,0q�6�   ���	#FFDE1B1B.��6�   •6�   �� 5D      -	   5        $ cameraLoad_btn��5D   %   $��6��6Z   $��2��6g   $��5,0,5,0q�6{   $��25��6�   .��6�   Load camera6�   �� 5E      $��7��6$   $��12(�61   $
��Bold"�6?   .��6Q   Socket6Y   �� 5F      -
   5        $��socketStatus_txt��5F   $   $��7��6<   $��12(�6I   $��1��6W   $
��Bold"�6g   $��Center=�6y   $��Right=�6�   $��0,0,5,0q�6�   ���	#FFDE1B1B.��6�   •6�   �� 5G      -   5        $ netConnect_btn��5G   %   $��8��6Z   $��0��6g   $��5,0,5,0q�6w   $��25��6�   .��6�   	Connect6�   �� 5H      -   5        $ netDisconnect_btn��5H   %   $��8��6`   $��1��6m   $��5,0,5,0q�6}   $��25��6�   .��6�   
Disconnect6�   5I      5J      5K      � 5L      $��1��6   .+�5M      �� $
��View��6   .��5N      � .+�5O      �� -   5        $��image��5O        J� RenderTransformOrigin$ 0.5,0.5&�6-    K� ZIndex$ 5��6M   z�5Q      V� 5R   "   .~�5S   &   �� �� 5T   &   �� 5U   &    �� Angle$	 180Q�66   T� 5V   &   5W   #   5Y      �� 5Z      $��Center=�6    $��Center=�6=   $��14(�6X   $ 2��6f   .��6w   Waiting for connection...6�   5[      5\      5]      � 5^      $��2��6    5_      �� 5`      �� 5a      $��20 �6(    5c      �� 5d      6-   �� 5e      $��50 �6+   68   .+�5g      �� $��SSH Terminal��6   $��0��65   $��2��6B   .��5h      �� 
�������.��5i      �� -   5         ��Name$ tblock��5i   (   	 �� Padding$	 5,5,0,0q�68   $��Ready.��6J   
���   ����	#FF04FF3D6k   5j      5k      �� 5l      -   5        $ command��5l   &   $�� ��6   $��1��67   $��20��6D   �� 5m      -   5        $��SendBtn��5m   :   $��1��6   $��1��6-   $��20��6_   .��6k   Send6q   5p      5q      � 5r      $��1��6    5s      �� 5t      �� 5u      .+�5w      � $��0��6   .+�5x      �� $��Eyes Control��6   .��5y      �  5z      �� 5{   "   �� 5|   "   .+�5~      � $��0��6#   .+�5   "   �� -   5        $ btnLeft��5   )   $��<��6:   $
��Left=�6I   $��50��6d   $��
42,120,0,0q�6p   $	��Top=�6�   $��49��6�   6�   �� 5�   "   -   5        $ btnRight��5�   )   $��>��6;   $
��Left=�6J   $��50��6e   $��151,120,0,0q�6q   $	��Top=�6�   $��49��6�   6�   �� 5�   "   -   5        $ btnDown��5�   )   $��\/��6:   $
��Left=�6G   $��50��6b   $��
96,174,0,0q�6n   $	��Top=�6�   $��50��6�   6�   �� 5�   "   -   5        $ btnUp��5�   )   $��/\��68   $
��Left=�6E   $��50��6`   $��	96,67,0,0q�6l   $	��Top=�6   $��50��6�   6�   �� 5�   "   -   5        $ 	btnCenter��5�   )   $�� ��6<   $
��Left=�6G   $��50��6b   $��
96,120,0,0q�6n   $	��Top=�6�   $��50��6�   6�   �� 5�   "   -   5        $ 	btnLeftUp��5�   )   $�� ��6<   $
��Left=�6G   $��25��6b   $��	66,90,0,0q�6n   $	��Top=�6�   $��25��6�   6�   �� 5�   "   -   5        $ btnLeftDown��5�   )   $�� ��6>   $
��Left=�6I   $��25��6d   $��
66,174,0,0q�6p   $	��Top=�6�   $��25��6�   6�   �� 5�   "   -   5        $ btnRightDown��5�   )   $�� ��6?   $
��Left=�6J   $��25��6e   $��151,174,0,0q�6q   $	��Top=�6�   $��25��6�   6�   �� 5�   "   -   5        $ 
btnRightUp��5�   )   $�� ��6=   $
��Left=�6H   $��25��6c   $��
151,90,0,0q�6o   $	��Top=�6�   $��25��6�   6�   �� 5�   "   -   5        
 ��Name$
 sliderY��5�   )   $
��Left=�6:   $��
205,90,0,0q�6U   $	��Top=�6i   $��23��6�    �� Orientation$ Vertical=�6�   $	��109��6�    � SmallChange$ 10Q�6�    � LargeChange$ 10Q�6�    � Maximum$	 140Q�6�    � Minimum$ 40Q�6�    � Value$ 90Q�6�   6�   �� 5�   "   -   5        $
 sliderX��5�   )   $
��Left=�6:   $��	61,20,0,0q�6U   $	��Top=�6h   $	��120��6�   $	 140Q�6�   $ 40Q�6�   $ 90Q�6�   $ 10Q�6�   $ 10Q�6�   $ 0.475,1.388&�6�   z�5�   &   V� 5�   *   .~�5�   .   �� �� 5�   .   �� 5�   .   $	 180Q�6>   T� 5�   .    T� X$ 6Q�6A    T� Y$ -51.504Q�6G   5�   +   5�   #   5�      � 5�      $��1��6#   .+�5�   "   �� -   5        $��Mrkni��5�   )   $
��Left=�69   $��
42,111,0,0q�6T   $	��Top=�6h   $	��158��6�   �� 5�   "   -   5        $��Zavři��5�   )   $
��Left=�69   $��	42,86,0,0q�6T   $	��Top=�6g   $��77��6�   �� 5�   "   -   5        $��Otevři��5�   )   $
��Left=�6:   $��
124,86,0,0q�6U   $	��Top=�6i   $��76��6�   �� 5�   "   -   5        $��Znechucení��5�   )   $
��Left=�6>   $��	42,61,0,0q�6Y   $	��Top=�6l   $	��158��6�   �� 5�   "   -    5        $��Překvapený��5�   )   $
��Left=�6>   $��	42,36,0,0q�6Y   $	��Top=�6l   $	��158��6�   �� 5�   "   -!   5        $��Mrkni jedním okem��5�   )   $
��Left=�6E   $��	42,11,0,0q�6`   $	��Top=�6s   $	��158��6�   �� 5�   "   -"   5        $��Závist��5�   )   $
��Left=�6:   $��
42,136,0,0q�6U   $	��Top=�6i   $	��158��6�   �� 5�   "   -#   5        $��Lobotmie��5�   )   $
��Left=�6<   $��
42,161,0,0q�6W   $	��Top=�6k   $	��158��6�   5�      5�      5�      5�      � 5�      $��1��6   .+�5�      .  (npcook.Terminal.Controls.TerminalControl  -$   5          Name$ terminalControl��5�   +   $��Consolas)�6D   $
|�True��6Z   $��13(�5�      
���   �$��1��6@   $
��Auto��6M   $
��Auto��6Z   $��Stretch=�5�      $��Stretch=�6=    J� SnapsToDevicePixels$
 True��6Y    � FocusVisualStyle 5�   k   Z� 5�      5�      5�      5�      5�      5�      