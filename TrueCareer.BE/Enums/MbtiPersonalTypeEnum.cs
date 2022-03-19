using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
    public class MbtiPersonalTypeEnum
    {
        public static GenericEnum ISTJ = new GenericEnum { Id = 1, Code = "ISTJ", Name = "Người trách nhiệm", Value = "Đây là nhóm tính cách phổ biến nhất, họ hướng đến chân lý, tôn trọng sự thật và có xu hướng tiếp thu nhiều thông tin và ghi nhớ chúng trong một thời gian dài." };
        public static GenericEnum ISFJ = new GenericEnum { Id = 2, Code = "ISFJ", Name = "Người nuôi dưỡng", Value = "Đây là nhóm tính cách vị tha nhất, do đó họ thường tìm kiếm con đường sự nghiệp trong lĩnh vực học thuật, y tế, công tác xã hội hoặc tư vấn; họ cũng tỏa sáng ở các vị trí hành chính và văn phòng, hoặc thậm chí trong các lĩnh vực hơi bất ngờ như thiết kế nội thất." };
        public static GenericEnum ISFP = new GenericEnum { Id = 3, Code = "ISFP", Name = "Người nghệ sĩ", Value = "Họ thường được liên hệ với tính tự phát và không thể đoán trước nhất trong tất cả các loại tính cách hướng nội, vì vậy nét đặc trưng nổi bật của họ là sự thay đổi." };
        public static GenericEnum ISTP = new GenericEnum { Id = 4, Code = "ISTP", Name = "Nhà kỹ thuật", Value = "Nhóm tính cách này sở hữu nhiều đặc điểm thú vị, họ thường suy nghĩ rất hợp lý và logic, nhưng cũng có thể khiến mọi người ngạc nhiên khi đột nhiên trở nên tự phát và nhiệt tình hơn." };
        public static GenericEnum INFP = new GenericEnum { Id = 5, Code = "INFP", Name = "Người lý tưởng hóa", Value = "Nhóm tính cách này thường được coi là bình tĩnh và dè dặt, tuy nhiên, ngọn lửa và niềm đam mê bên trong họ rất lớn, không giống như các loại tính cách khác, họ thực sự tình cảm và có lòng trắc ẩn cao." };
        public static GenericEnum INFJ = new GenericEnum { Id = 6, Code = "INFJ", Name = "Người che chở", Value = "Nhóm tính cách này thường có những quan điểm mạnh mẽ, đặc biệt là khi liên quan đến các vấn đề mà họ cho là thực sự quan trọng trong cuộc sống, vì vậy nếu nhóm INFJ đấu tranh vì điều gì đó, lý do chính là họ tin vào lý tưởng của chính mình." };
        public static GenericEnum INTJ = new GenericEnum { Id = 7, Code = "INTJ", Name = "Nhà khoa học", Value = "Nhóm này thường được xem là rất thông minh và khó hiểu một cách bí ẩn, vì vậy họ thường tỏa ra sự tự tin, dựa trên kho kiến thức rộng lớn của họ bao gồm nhiều lĩnh vực và khía cạnh khác nhau." };
        public static GenericEnum INTP = new GenericEnum { Id = 8, Code = "INTP", Name = "Nhà tư duy", Value = "Họ thích những học thuyết và tin rằng mọi thứ đều có thể được phân tích và cải thiện, vì vậy họ không quan tâm đến thế giới trần tục và những điều thực tế khác – họ nghĩ rằng nó ít thú vị hơn so với những ý tưởng hoặc hành trình theo đuổi kiến thức." };
        public static GenericEnum ENFJ = new GenericEnum { Id = 9, Code = "ENFJ", Name = "Người cho đi", Value = "Những người thuộc nhóm tính cách này có sức ảnh hưởng lớn bởi vì họ thường rất lôi cuốn và có tài hùng biện. Họ chăm sóc mọi người một cách chân thành, dễ dàng truyền đạt ý tưởng và ý kiến của họ cho mọi người xung quanh." };
        public static GenericEnum ENFP = new GenericEnum { Id = 10, Code = "ENFP", Name = "Người truyền cảm hứng", Value = "Họ thường rất tò mò, duy tâm và khá bí ẩn vì họ tìm kiếm ý nghĩa và thực sự quan tâm đến động cơ của người khác, vì vậy họ thấy cuộc sống rất rộng lớn và có nhiều câu đố chưa được giải mã mà trong đó mọi thứ đều liên hệ với nhau." };
        public static GenericEnum ENTJ = new GenericEnum { Id = 11, Code = "ENTJ", Name = "Nhà điều hành", Value = "Nhóm tính cách này thường rất lôi cuốn, lý trí và nhạy bén vì họ rất giỏi trong việc lãnh đạo và truyền cảm hứng cho người khác, vì vậy nhóm ENTJ có khả năng lãnh đạo tốt nhất trong tất cả các loại tính cách và họ tin rằng nếu có quyết tâm, mọi thứ đều có thể." };
        public static GenericEnum ENTP = new GenericEnum { Id = 12, Code = "ENTP", Name = "Người nhìn xa", Value = "Nhóm tính cách này rất nhanh nhạy và độc đáo, điều này mang lại cho họ một lợi thế lớn trong các cuộc tranh luận, các lĩnh vực học thuật và chính trị. Tuy nhiên họ cũng có xu hướng làm rất tốt trong nhiều lĩnh vực khác đòi hỏi phải sẵn sàng thách thức các ý tưởng hiện có và tổ chức nhiều cuộc tranh luận." };
        public static GenericEnum ESFJ = new GenericEnum { Id = 13, Code = "ESFJ", Name = "Người quan tâm", Value = "Nhóm tính cách này là những người thực tế, vị tha, giỏi làm việc nhóm, truyền thống và làm hết sức mình để hỗ trợ và bảo vệ lẽ phải và quyền lợi của họ, vì vậy họ có xu hướng rất tận tụy ngay cả khi họ đóng vai trò là người chủ trì của một bữa tiệc hoặc một nhân viên xã hội." };
        public static GenericEnum ESFP = new GenericEnum { Id = 14, Code = "ESFP", Name = "Người trình diễn", Value = "Họ thích là trung tâm của sự chú ý và cũng thích những điều đơn giản nhất. Sự vui vẻ và bản chất nồng nhiệt của họ thường rất hấp dẫn người khác, vì vậy họ không bao giờ cạn ý tưởng và sự tò mò của họ là vô hạn." };
        public static GenericEnum ESTJ = new GenericEnum { Id = 15, Code = "ESTJ", Name = "Người giám hộ", Value = "Họ là những người thiên về nguyên tắc, truyền thống, sự ổn định và họ cảm thấy cần phải gắn kết với điều gì đó – đó có thể là một gia đình, một cộng đồng hoặc một nhóm xã hội khác; vì vậy họ thích sự tổ chức của người khác và đảm bảo rằng họ sẽ tuân thủ các quy tắc truyền thống mà được ban hành bởi những người có thẩm quyền." };
        public static GenericEnum ESTP = new GenericEnum { Id = 16, Code = "ESTP", Name = "Người thực thi", Value = "Những người thuộc nhóm này rất có tính tập thể, tự phát, thẳng thắn, thích hành động và luôn đi thẳng vào cốt lõi của vấn đề, vì vậy họ không thích những cuộc tranh luận lý thuyết hay suy nghĩ về tương lai - họ chỉ tập trung vào thời điểm hiện tại và nỗ lực hết mình cho những thứ họ thích." };
        public static List<GenericEnum> MbtiPersonalTypeEnumList = new List<GenericEnum>
        {
            ISTJ, ISFJ, ISFP, ISTP, INFP, INFJ, INTJ, INTP, ENFJ, ENFP, ENTJ, ENTP, ESFJ, ESFP, ESTJ, ESTP
        };
    }
}
