   M S B A M L   `   `   `  ����   Whack-A-Mole    WhackAMole.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006        backgroundImage#���     � Title$  Whack-A-Mole��$	��550��$
��1000�� � 	Resources   NoMouseOverButtonTemplate �� 
TargetType �� Triggers P� Property P� Value �� Value �� Property  	imgSource �� ImageSource	 �� Opacity��%  &	       &	 �     &	  �     �� " ��.R��� #��z��#��z��#��z��._��� #��z��#��z�� P�  � {�$ False��. ��� # Z� � ���� # Z
� � ����QMC:\Users\Jesper H. Hansen\Desktop\Whack-A-Mole\Whack-A-Mole\Sprites\Grass.png���� # �  $		 0.9Q�.��� 
 � RowDefinitions
 �� $
��Auto ��� $��* �.+��� $��0�� �� Orientation$ 
Horizontal=�$��Center=����	#AA000000.+��� $��Score: ��$��Center=�
�������$��24(��� -    ��Name$ ScoreTextBlock��$��0��$��Center=�
�������$��24(��� $��  Time: ��$��Center=�
�������$��24(��� -   $ TimeTextBlock��$	��300��$��Center=�
�������$��24(��� $��
  Health: ��$��Center=�
�������$��24(��� -   $ HitPointsValue��$��3��$��Center=�
�������$��24(�A� $��1�� A� Rows$ 3�� A� Columns$ 3��
������ .+��� -    ��Name$ Mole1��$�� ��$��0q�
������ #��[ �� -   $ Mole2��$�� ��$��0q�
������ #��[ �� -   $ Mole3��$�� ��$��0q�
������ #��[ �� -   $ Mole4��$�� ��$��0q�
������ #��[ �� -   $ Mole5��$�� ��$��0q�
������ #��[ �� -	   $ Mole6��$�� ��$��0q�
������ #��[ �� -
   $ Mole7��$�� ��$��0q�
������ #��[ �� -   $ Mole8��$�� ��$��0q�
������ #��[ �� -   $ Mole9��$�� ��$��0q�
������ #��[ � -    �Name$ HitPanel��$y�	Collapsed=����	#AAFF0000 J� Opacity$	 0.6Q�$��2�� J� IsHitTestVisible$
 True��� -   $ GameOverPanel��$y�	Collapsed=����	#AA000000$��2��$
 True��.+��� $��Center=�$��Center=�.+��� -   $ GameOverText��$��	Game Over��$��48(�
������� �� TextAlignment$ Center=�$��0,20,0,0q��� -   $ FinalScoreText��$��24(�
�������$ Center=�$��	0,10,0,20q��� -   $ HighScoreText��$�� ��$��24(�
�������$ Center=�$��0, 10, 0, 20q��� -   $ TryAgainButton��$��Return To Menu��$��24(�$	��200��$��50��� -   $ StartupMenu�����	#AA000000$y�Visible=�$��2��$
 True��.+��� $��Center=�$��Center=�.+��� $��Welcome to Whack-A-Mole!��$��48(�
�������$ Center=��� $L��FHit as many moles as you can without hitting the cats or an empty box.��$��24(�
�������$ Center=�$��0,20,0,0q�$
��Wrap=��� $d��^Moles give 1 point, Golden moles give 3, Cats take a point away, and an empty box does nothing��$��24(�
�������$ Center=�$��0,20,0,0q�$
��Wrap=��� -   $ StartButton��$��
Start Game��$��24(�$	��200��$��50��$��0,20,0,0q��� -   $ Settings��$��Settings��$��24(�$	��200��$��50��$��0,20,0,0q�� -   $ SettingsMenu�����	#AA000000$y�	Collapsed=�$��2��$
 True��.+��� $��Center=�$��Center=�.+��� $��
BGM Volume��$��40(�
�������$ Center=��� -    ��Name$ 	BGMSlider�� � Minimum$ 0Q� � Maximum$	 100Q� � Value$ 50Q��� $��
SFX Volume��$��40(�
�������$ Center=��� -   $ 	SFXSlider��$ 0Q�$	 100Q�$ 50Q��� $��Difficulty:��$��40(�
�������$ Center=��� -   $��
Difficulty��$��40��$��28(�$��Center=�  �� 
TargetType��%1   '��       �� " ��.���  � ��$ Center=�.@��� $
��Easy��$��30(��� $��Medium��$��30(��� $
��Hard��$��30(��� $"��Game Time (Standard is 300):��$��40(�
�������$ Center=��� -    ��Name$ GameTimeValue��$	��200�� �� TextAlignment$ Center=�$��26(�$	��300���� $��Health (Standard is 3):��$��40(�
�������$ Center=��� -   $ UserHitPoints��$	��200��$ Center=�$��26(�$��3���� -   $ Return��$��Save and Return��$��24(�$	��200��$��50��$��0,20,0,0q�