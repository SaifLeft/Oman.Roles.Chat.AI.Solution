namespace MauiKit.Views;

public partial class FormsPage : BasePage
{
	public FormsPage()
	{
		InitializeComponent();
	}

    async void VideoBackgroundLogin_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new VideoBackgroundLoginPage());
    }

    async void VideoBackgroundSignUp_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new VideoBackgroundSignUpPage());
    }

    async void BackgroundGradientLogin_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new BackgroundGradientLoginPage());
    }

    async void BackgroundGradientSignUp_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new BackgroundGradientSignUpPage());
    }

    async void FullBackgroundLogin_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new FullBackgroundLoginPage());
    }

    async void FullBackgroundSignUp_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new FullBackgroundSignUpPage());
    }

    async void Login_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushModalAsync(new LoginPage());
    }

    async void SignUp_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SignUpPage());
    }

    async void SimpleLogin_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SimpleLoginPage());
    }

    async void SimpleSignup_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SimpleSignUpPage());
    }

    async void ForgotPassword_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

    async void PasswordVerification_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PasswordVerificationPage());
    }

    async void ChangePassword_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChangePasswordPage());
    }

}