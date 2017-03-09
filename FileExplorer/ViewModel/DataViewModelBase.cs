using FileExplorer.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace FileExplorer.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">child class</typeparam>
    interface IOperation<T>
    {
        bool GetIsAddEnabled();
        bool GetIsEditEnabled();
        bool GetIsRemoveSelfEnabled();
        bool GetIsRemoveEnabled();

        /// <summary>
        /// Add child
        /// </summary>
        void Add();

        /// <summary>
        /// Edit child
        /// </summary>
        void Edit();

        /// <summary>
        /// Remove self by parent
        /// </summary>
        void Remove();

        /// <summary>
        /// Remove child
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);
    }

    /// <summary>
    /// 结构化数据基类
    /// </summary>
    /// <typeparam name="T"> child class</typeparam>
    public abstract class DataViewModelBase<T> : ViewModelBase, IOperation<T>
        where T : class
    {
        #region Props 

        private ObservableCollection<T> items = new ObservableCollection<T>();
        public ObservableCollection<T> Items
        {
            get { return items; }
            protected set { SetProperty(ref items, value, "Items", "ItemsView"); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                SetProperty(ref title, value, "Title");
            }
        }

        private ICollectionView itemsView;
        public ICollectionView ItemsView
        {
            get { return itemsView; }
            protected set
            {
                SetProperty(ref itemsView, value, "ItemsView");
            }
        }

        public T SelectedItem { get { return ItemsView.CurrentItem as T; } }

        #endregion

        #region Commands

        public ICommand AddCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) =>
                    {
                        this.Add();
                    }
                };
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) =>
                    {
                        this.Edit();
                    }
                };
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) =>
                    {
                        this.Remove();
                    }
                };
            }
        }

        #endregion

        public DataViewModelBase()
        {
            this.ItemsView = CollectionViewSource.GetDefaultView(this.Items);
        }

        #region Operation<T>

        public virtual bool GetIsAddEnabled() { return true; }
        public virtual bool GetIsEditEnabled() { return true; }
        public virtual bool GetIsRemoveSelfEnabled() { return true; }
        public virtual bool GetIsRemoveEnabled() { return true; }

        public virtual void Add() { }

        public virtual void Edit() { }

        public virtual void Remove() { }

        public virtual void Remove(T item)
        {
            if (item.IsNull())
            {
                return;
            }
            if (this.Items.Contains(item))
            {
                this.Items.Remove(item);
            }
        }

        #endregion
    }

    /// <summary>
    /// 结构化数据基类带Parent
    /// </summary>
    /// <typeparam name="T"> child class</typeparam>
    /// <typeparam name="P">parent class</typeparam>
    public abstract class DataViewModelBase<P, T> : DataViewModelBase<T>
        where T : class
        where P : class
    {
        public P Parent { get; protected set; }

        public DataViewModelBase(P parent) : base()
        {
            if (parent.IsNull())
            {
                throw new InvalidOperationException();
            }
            this.Parent = parent;
        }
    }
}
