using Extractors.Contracts.Enums;

namespace Extractors.Contracts.Types {
    public class DocumentBase {
        public DocumentType DocumentType;
        public bool Success;

        public virtual string LogMessage() {
            return string.Empty;
        }
    }
}
