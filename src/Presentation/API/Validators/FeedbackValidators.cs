using API.Controllers;
using FluentValidation;

namespace API.Validators
{
    /// <summary>
    /// Validator for chat feedback requests
    /// </summary>
    public class ChatFeedbackRequestValidator : AbstractValidator<ChatFeedbackRequest>
    {
        public ChatFeedbackRequestValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty().WithMessage("ConversationIdRequired");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("RatingOutOfRange");

            RuleFor(x => x.Comments)
                .MaximumLength(1000).WithMessage("CommentsTooLong")
                .When(x => !string.IsNullOrEmpty(x.Comments));
        }
    }

    /// <summary>
    /// Validator for app feedback requests
    /// </summary>
    public class AppFeedbackRequestValidator : AbstractValidator<AppFeedbackRequest>
    {
        public AppFeedbackRequestValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("RatingOutOfRange");

            RuleFor(x => x.Category)
                .MaximumLength(50).WithMessage("CategoryTooLong")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.Comments)
                .MaximumLength(1000).WithMessage("CommentsTooLong")
                .When(x => !string.IsNullOrEmpty(x.Comments));
        }
    }
} 