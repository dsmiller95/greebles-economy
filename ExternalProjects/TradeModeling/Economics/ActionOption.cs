using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeModeling.Economics
{
    public class ActionOption<OptionInfo>
    {
        public OptionInfo info;
        private Action option;

        public ActionOption(OptionInfo info, Action onExecute)
        {
            this.info = info;
            this.option = onExecute;
        }

        public void Execute()
        {
            this.option();
        }

        public ActionOption<T1> Then<T1>(Func<OptionInfo, T1> infoGenerator, Action<T1, OptionInfo> execute)
        {
            var newInfo = infoGenerator(this.info);
            return new ActionOption<T1>(newInfo, () =>
            {
                this.Execute();
                execute(newInfo, this.info);
            });
        }
        public ActionOption<T1> Then<T1>(Func<OptionInfo, T1> infoGenerator, Action<T1> execute = null)
        {
            var newInfo = infoGenerator(this.info);
            return new ActionOption<T1>(newInfo, () =>
            {
                this.Execute();
                execute?.Invoke(newInfo);
            });
        }
    }
}
