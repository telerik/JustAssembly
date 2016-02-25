using System;
using System.Linq;
using System.Threading;
using JustDecompile.External.JustAssembly;
using Microsoft.Practices.Prism.ViewModel;

namespace JustAssembly.Interfaces
{
    abstract class TabSourceItemBase : NotificationObject, ITabSourceItem
    {
        private bool isBusy;

        protected string header;

        protected int progress;

        private bool isIndeterminite;

        private uint totalFileCount;

        private string loadingMessage;

        private CancellationTokenSource cancellationTokenSource;

        public abstract void OnProjectFileGenerated(IFileGeneratedInfo args);

        public abstract string ToolTip { get; }

        public uint TotalFileCount
        {
            get
            {
                return this.totalFileCount;
            }
            set
            {
                if (this.totalFileCount != value)
                {
                    this.totalFileCount = value;
                    RaisePropertyChanged("TotalFileCount");
                }
            }
        }

        public string LoadingMessage
        {
            get
            {
                return this.loadingMessage;
            }
            set
            {
                if (this.loadingMessage != value)
                {
                    this.loadingMessage = value;
                    RaisePropertyChanged("LoadingMessage");
                }
            }
        }

        /// <summary>
        /// Clears up the state of the progress tracking properties, so that they can be reused the next time they're needed.
        /// </summary>
        public void Completed()
        {
            this.IsBusy = false;
            this.Progress = 0;
            this.TotalFileCount = 0;
        }

        public CancellationToken GetCanellationToken()
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            return this.cancellationTokenSource.Token;
        }

        public void CancelProgress()
        {
            if (cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }
            this.Completed();
        }

        public abstract TabKind TabKind { get; }

        public virtual string Header
        {
            get
            {
                return this.header;
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;

                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminite;
            }
            set
            {
                if (this.isIndeterminite != value)
                {
                    this.isIndeterminite = value;
                    this.RaisePropertyChanged("IsIndeterminate");
                }
            }
        }

        public int Progress
        {
            get
            {
                if (TotalFileCount == 0)
                {
                    return 0;
                }
                return (this.progress * 100) / (int)TotalFileCount;
            }
            set
            {
                int oldProgress= this.Progress;
                if (this.progress != value)
                {
                    this.progress = value;
                }
                if (oldProgress != this.Progress)
                {
                    // update the progress only when enough items were generated, so that the bar will increase with at least 1%
                    // (i.e. dont raise property changed for each item, when generating 5000 files, instead raise it on every 50th)
                    RaisePropertyChanged("Progress");
                }
            }
        }

        public abstract void LoadContent();

		public virtual void ReloadContent()
		{
			this.LoadContent();
		}

        public virtual void Dispose()
        {
        }
    }
}
