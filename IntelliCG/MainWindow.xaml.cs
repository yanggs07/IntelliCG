﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using IntelliCG.Cheat;
using MemoLib;

namespace IntelliCG
{

    public partial class MainWindow
    {
        public static List<GameWindow> GameWindows { get; } = new List<GameWindow>();
        private readonly NotifyIcon _notifyIcon;


        public MainWindow()
        {

            InitializeComponent();
            Hide();


            var contextMenu = new ContextMenu();

            contextMenu.MenuItems.Clear();



            var menuItemShowHide = new MenuItem("Show/Hide");
            menuItemShowHide.Click += MenuItemShowHide_Click;
            contextMenu.MenuItems.Add(menuItemShowHide);

            var menuItemUnlimited = new MenuItem("Unlimited");
            menuItemUnlimited.Click += MenuItemUnlimited_Click;
            contextMenu.MenuItems.Add(menuItemUnlimited);

            var menuItemRefresh = new MenuItem("Refresh");
            menuItemRefresh.Click += MenuItemRefresh_Click;
            contextMenu.MenuItems.Add(menuItemRefresh);

            var menuItemExit = new MenuItem("Exit");
            menuItemExit.Click += MenuItemExit_Click;
            contextMenu.MenuItems.Add(menuItemExit);



            _notifyIcon = new NotifyIcon
            {
                BalloonTipTitle = @"IntelliCG",
                BalloonTipText = @"IntelliCG is running here.", //设置程序启动时显示的文本
                BalloonTipIcon = ToolTipIcon.Info,
                Text = @"IntelliCG",//最小化到托盘时，鼠标点击时显示的文本
                Icon = new Icon("icon.ico"),//程序图标
                Visible = true,
            };
            _notifyIcon.ShowBalloonTip(0);
            _notifyIcon.MouseDoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.ContextMenu = contextMenu;

            Refresh();
        }
        
        private  void MenuItemUnlimited_Click(object sender, EventArgs e)
        {
            try
            {
                CloseObject.CloseHandles();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private  void MenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }
        private void MenuItemShowHide_Click(object sender, EventArgs e)
        {
            ShowHide();
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            //Show();
            //WindowState = WindowState.Normal;
            //_notifyIcon.Visible = false;
            Refresh();
        }

        private void Refresh()
        {

            var processes = Memo.GetProcess("cg_se_3000");

            //var windows = Window.FindAllWindows();

            //Window.TileWindows(windows);

            //如果窗口不在打开新窗口
            foreach (var process in processes)
            {
                //窗口中不包含新发现的游戏
                if (GameWindows.Exists(w => w.Cg.Memo.Process.Id == process.Id)) continue;
                var newGame = new CrossGate(new Memo(process));
                new GameWindow(newGame).Show();
            }

            //关闭游戏不存在的窗口
            for (var i = GameWindows.Count - 1; i >= 0; i--)
            {
                if (GameWindows[i].Cg.Memo.Process.HasExited){
                    GameWindows[i].Close();
                }
            }


            var count = GameWindows.Count;
            if (SystemParameters.PrimaryScreenWidth > 3000.00)
            {
                for (var i = 0; i < count; i++)
                {
                    GameWindows[i].MoveGameWindowAndSelf(640 * (i % 5), 535 * (i / 5));
                }
            }
            else if (SystemParameters.PrimaryScreenWidth > 2500.00)
            {
                for (var i = 0; i < count; i++)
                {
                    GameWindows[i].MoveGameWindowAndSelf(640 * (i % 4)-3, 535 * (i / 4));
                }
            }
            else if (SystemParameters.PrimaryScreenWidth >= 1920.00)
            {
                for (var i = 0; i < count; i++)
                {
                    GameWindows[i].MoveGameWindowAndSelf(640 * (i % 3), 400 * (i / 3));
                }
            }
            else if (SystemParameters.PrimaryScreenWidth < 1920.00)
            {
                for (var i = 0; i < count; i += 3)
                {
                    GameWindows[i].MoveGameWindowAndSelf(0, 400 * (i / 3));
                    GameWindows[i].MoveGameWindowAndSelf(Convert.ToInt32(SystemParameters.PrimaryScreenWidth - 640.0) / 2, 400 * (i / 3));
                    GameWindows[i].MoveGameWindowAndSelf(Convert.ToInt32(SystemParameters.PrimaryScreenWidth - 640.0), 400 * (i / 3));
                }
            }
            _notifyIcon.BalloonTipText = @"Game Binding Refreshed.";
            _notifyIcon.ShowBalloonTip(0);
        }

        
        private void ShowHide()
        {
            var show=GameWindows.Find(gw => gw.WindowState == WindowState.Minimized)!=null;
            foreach (var gameWindow in GameWindows)
            {
                if (show)
                {
                    gameWindow.WindowState = WindowState.Normal;
                    gameWindow.Cg.Memo.ShowHideWindow(true);
                }
                else
                {
                    gameWindow.WindowState = WindowState.Minimized;
                    gameWindow.Cg.Memo.ShowHideWindow(false);
                }
            }
        }


        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            for (var i = GameWindows.Count-1; i>0; i--)
            {
                GameWindows[i].Close();
            }
        }
    }
}

