namespace Infrastructure.Helpers;
public class FormatReviews
{
    public static string Reviews(int reviews)
    {
        if (reviews >= 1000 && reviews < 1000000)
        {
            return (reviews / 1000).ToString("0K");
        }
        else
        {
            return reviews.ToString();
        }
    }
}
