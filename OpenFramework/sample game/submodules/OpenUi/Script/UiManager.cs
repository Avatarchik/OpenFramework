﻿using System;
using System.Collections.Generic;
using OpenUi;
using UnityEngine;
namespace OpenUi
{
    public class UiManager<TWin, TMod>
        where TWin : struct, IConvertible
        where TMod : struct, IConvertible
    {
        #region Properties
        // private static UiManager<TWin, TMod> _instance;
        // public static UiManager<TWin, TMod> instance
        // {
        //     get
        //     {
        //         if (_instance == null)
        //         {
        //             _instance = GameObject.FindObjectOfType<UiManager<TWin, TMod>>();
        //             if (_instance == null)
        //             {
        //                 var t = new GameObject("ui-manager");
        //                 _instance = t.AddComponent<UiManager<TWin, TMod>>();
        //             }
        //         }
        //         return _instance;
        //     }
        // }
        public Canvas canvas
        {
            get
            {
                if (_canvas == null)
                {
                    var t = Resources.Load<Canvas>(_setting.canvasPath);
                    _canvas = GameObject.Instantiate(t);
                    // _canvas.transform.SetParent(transform, false);
                }
                return _canvas;
            }
        }
        private Canvas _canvas;
        #endregion

        #region Fields
        [SerializeField] private TWin initialMenu;
        private List<Window<TWin, TMod>> windowPrefabs;
        private List<Window<TWin, TMod>> windowList;
        private List<Modal<TMod>> modalPrefabs;
        private List<FormButton> formButtonPrefabs;
        private Window<TWin, TMod> currentWindow;
        private UiManagerSetting _setting;

        public UiManager(UiManagerSetting setting)
        {
            _setting = setting;
        }
        #endregion

        #region Methods
        public void Init()
        {
            LoadService();
            // ChangeWindow(initialMenu);
        }

        void LoadService()
        {
            windowPrefabs = new List<Window<TWin, TMod>>();
            windowList = new List<Window<TWin, TMod>>();
            modalPrefabs = new List<Modal<TMod>>();
            formButtonPrefabs = new List<FormButton>();
            windowPrefabs.AddRange(Resources.LoadAll<Window<TWin, TMod>>(_setting.windowPath));
            modalPrefabs.AddRange(Resources.LoadAll<Modal<TMod>>(_setting.modalPath));
            formButtonPrefabs.AddRange(Resources.LoadAll<FormButton>(_setting.buttonPath));
        }

        public void ChangeWindow(TWin windowType)
        {
            if (currentWindow != null) currentWindow.Hide();
            Window<TWin, TMod> window;
            window = windowList.Find(x => EqualityComparer<TWin>.Default.Equals(x.windowType, windowType));

            // if first time showing window
            if (window == null)
            {
                Window<TWin, TMod> windowPrefab = windowPrefabs.Find(x => EqualityComparer<TWin>.Default.Equals(x.windowType, windowType));
                if (windowPrefab != null)
                {
                    window = GameObject.Instantiate(windowPrefab);
                    window.transform.SetParent(canvas.transform, false);
                    // window.transform.SetAsLastSibling();
                    windowList.Add(window);
                }
                else
                {
                    Debug.LogError("Could not find window with " + windowType + " type in Resources/" + _setting.windowPath + " path.");
                    return;
                }
            }
            currentWindow = window;
            window.Show();
        }

        public Modal<TMod> ShowModal(TMod modalType)
        {
            Modal<TMod> modal;
            modal = currentWindow.GetModal(modalType);
            // if first time showing modal
            if (modal == null)
            {
                Modal<TMod> modalPrefab = modalPrefabs.Find(x => EqualityComparer<TMod>.Default.Equals(x.modalType, modalType));
                if (modalPrefab != null)
                {
                    modal = GameObject.Instantiate(modalPrefab);
                    modal.transform.SetParent(currentWindow.transform, false);
                    // modal.transform.SetAsLastSibling();
                }
                else
                {
                    Debug.LogError("Could not find modal with " + modalType + " type in Resources/" + _setting.modalPath + " path.");
                    return null;
                }
            }
            currentWindow.AddModal(modal);
            modal.Show();
            return modal;
        }

        public Modal<TMod> HideModal(TMod modalType)
        {
            Modal<TMod> modal;
            modal = currentWindow.GetModal(modalType);
            if (modal != null)
            {
                modal.Hide();
                currentWindow.RemoveModal(modal);
            }
            return modal;
        }

        internal FormButton GetButtonPrefab(FormButtonTypes formButtonType)
        {
            FormButton btnPrefab = formButtonPrefabs.Find(x => x.formButtonType == formButtonType);
            return btnPrefab;
        }
        #endregion
    }
}