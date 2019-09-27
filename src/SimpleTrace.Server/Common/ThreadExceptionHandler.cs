using System;
using System.Threading;
using System.Windows.Forms;
// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    public class ThreadExceptionHandler
    {
        #region how to use

        //ThreadExceptionHandler.HandleApplicationThreadException();

        #endregion

        public void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                //todo log ex?
                var result = ShowThreadExceptionDialog(e.Exception);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            catch
            {
                try
                {
                    MessageBox.Show("严重错误",
                        "程序出现严重错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        private DialogResult ShowThreadExceptionDialog(Exception ex)
        {
            string errorMessage =
                "Unhandled Exception:\n\n" +
                ex.Message + Environment.NewLine + Environment.NewLine +
                ex.GetType() + Environment.NewLine + Environment.NewLine +
                "Stack Trace:" + Environment.NewLine +
                ex.StackTrace;

            return MessageBox.Show(errorMessage,
                "发现了一个未处理的异常，要终止程序吗？",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);
        }
        
        public static void HandleApplicationThreadException()
        {
            var handler = new ThreadExceptionHandler();
            Application.ThreadException += handler.Application_ThreadException;
        }
    }
}