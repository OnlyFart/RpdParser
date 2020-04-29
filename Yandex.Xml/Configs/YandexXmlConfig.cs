namespace Yandex.Xml.Configs {
    /// <summary>
    /// Конфиг YandexXmlProvider'a
    /// </summary>
    public class YandexXmlConfig {
        /// <summary>
        /// Пользователь от которого будут выполняться запросы. Брать отсюда https://xml.yandex.ru/test/
        /// </summary>
        public string User { get; set; }
        
        /// <summary>
        /// Ключ пользователя от которого будут выполняться запросы. Брать отсюда https://xml.yandex.ru/test/
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Прокси-сервер через который будут отправлять запросы. Можно не указывать
        /// </summary>
        public string ProxyUrl { get; set; }
        
        /// <summary>
        /// Порт прокси-сервера. Можно не указывать
        /// </summary>
        public int ProxyPort { get; set; }
    }
}
