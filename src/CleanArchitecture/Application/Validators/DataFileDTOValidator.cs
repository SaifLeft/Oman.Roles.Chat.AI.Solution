using FluentValidation;
using Application.DTOs;

namespace Application.Validators
{
    public class DataFileDTOValidator : AbstractValidator<DataFileDTO>
    {
        public DataFileDTOValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("اسم الملف مطلوب")
                .MaximumLength(255).WithMessage("لا يمكن أن يتجاوز اسم الملف 255 حرفًا");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("محتوى الملف مطلوب");
        }
    }
}