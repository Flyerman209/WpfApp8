using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApp8
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            using (var context = new SnakeGameDBEntities1())
            {
                // Получаем список лучших результатов из базы данных
                List<HighScores> leaderboard = context.HighScores.OrderByDescending(s => s.Score).ToList();

                // Устанавливаем список лидеров в источник данных для DataGrid
                LeaderboardDataGrid.ItemsSource = leaderboard;
            }
        }
    }
}

