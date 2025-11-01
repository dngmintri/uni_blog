namespace frontend.Services

{
    public class PageTitleService
    {
        public event Action? OnChange;
        private string _title = "Uni Blog"; // Tiêu đề mặc định

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
