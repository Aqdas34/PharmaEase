using Microsoft.AspNetCore.SignalR;

namespace RealProject.Hubs
{
    public class MedicineHub : Hub
    {
        public async Task SendMedicineAddedNotification(string medicineName, decimal price)
        {
            await Clients.All.SendAsync("ReceiveNewMedicine", medicineName, price);
        }
    }
}
