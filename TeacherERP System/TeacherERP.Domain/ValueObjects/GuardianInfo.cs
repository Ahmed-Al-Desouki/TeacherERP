namespace TeacherERP.Domain.ValueObjects
{
    /// <summary>
    /// بيانات ولي أمر الطالب (Value Object). يُخزَّن في جدول المستخدمين نفسه.
    /// رقم الموبايل يصل هنا بعد تحويله للصيغة الدولية (+20...).
    /// </summary>
    public class GuardianInfo
    {
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public GuardianRelation Relation { get; private set; }

        /// <summary>منشئ فارغ مطلوب من قِبل Entity Framework Core</summary>
        private GuardianInfo() { }

        public static GuardianInfo Create(string name, string phoneNumber, GuardianRelation relation)
        {
            return new GuardianInfo
            {
                Name = name.Trim(),
                PhoneNumber = phoneNumber,
                Relation = relation
            };
        }
    }
}