using FluentValidation;
using Models.DTOs;

namespace Models.Validators
{
    public class UpdateFileInfoRequestDTOValidator : AbstractValidator<UpdateFileInfoRequestDTO>
    {
        public UpdateFileInfoRequestDTOValidator()
        {
            RuleFor(x => x.FileId)
                .NotEmpty().WithMessage("معرف الملف مطلوب")
                .GreaterThan(0).WithMessage("معرف الملف يجب أن يكون رقمًا صحيحًا موجبًا");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("لا يمكن أن يتجاوز العنوان 100 حرفًا");
        }
    }
}