using System;
using System.Threading;
using Core.Infrastructure.WindowsFsm;
using Core.MVP.Presenters;
using Game.Data.Dto;
using Game.MVP.Presentation.Services;
using Game.MVP.Presentation.Views;
using Game.Shared.Windows;

namespace Game.MVP.Presentation.Presenters
{
    public class MainUiPresenter : IPresenter
    {
        private readonly LevelService _levelService;
        private readonly MainUiView _view;
        private readonly IWindowFsm _windowFsm;
        private readonly Type _window = typeof(MainUi);

        private CancellationTokenSource _cts;

        public MainUiPresenter(
            MainUiView view, 
            LevelService levelService, 
            IWindowFsm windowFsm)
        {
            _view = view; 
            _levelService = levelService;
            _windowFsm = windowFsm;

            _levelService.UpdateStonesCount += UpdateStonesCount;
            _levelService.ShowProgressBar += ShowProgressBarAsync;
        }
        
        public void Enable()
        {
            _windowFsm.Opened += OnHandleOpenWindow;
            _windowFsm.Closed += OnHandleCloseWindow;
        }

        public void Disable()
        {
            _windowFsm.Opened -= OnHandleOpenWindow;
            _windowFsm.Closed -= OnHandleCloseWindow;
            
            _levelService.UpdateStonesCount -= UpdateStonesCount;
            _levelService.ShowProgressBar -= ShowProgressBarAsync;
            
            _cts?.Dispose();
            _cts = null;
        }

        private void OnHandleOpenWindow(Type window)
        {
            if(_window != window || _view == null) return;
            
            _view.StonesCounterObject.SetActive(false);
            _view.ProgressBarObject.SetActive(false);
            _view.Show();
        }
        
        private void OnHandleCloseWindow(Type window)
        {
            if(_window != window || _view == null) return;
            
            _view.Hide();
        }

        private void UpdateStonesCount(int count)
        {
            _view.StonesCounterObject.SetActive(count > 0);
            _view.StonesCountText.text = count.ToString();
        }

        private async void ShowProgressBarAsync(MagazineProgressDto dto, bool isAnimation)
        {
            _view.ProgressBarText.text = $"{dto.Current}/{dto.Max}";
            _view.ProgressBarObject.SetActive(dto.IsActive);
            float fill = (float) dto.Current / dto.Max;
            
            if (isAnimation)
            {
                try
                {
                    _cts = new CancellationTokenSource();
                    await _view.FillAnimation.DoFillAnimationAsync(fill, _cts.Token);
                }
                catch (OperationCanceledException e)
                {
                    _view.ProgressBarFillImage.fillAmount = fill;
                }
                finally
                {
                    _cts?.Dispose();
                    _cts = null;
                }
            }
            _view.ProgressBarFillImage.fillAmount = fill;
        }
    }
}
