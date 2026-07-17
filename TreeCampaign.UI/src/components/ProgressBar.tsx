interface ProgressSection {
  title: string;
  amount: number;
  color: string;
}

interface ProgressBarProps {
  total?: number;
  parts: ProgressSection[];
  onClick?: () => any;
}

export default ({ parts, total, onClick }: ProgressBarProps) => {
  const totalCalc = total ?? parts.reduce((prev, part) => part.amount + prev, 0);
  return (
    <div className="w-full flex items-center">
    <div className="w-full h-6 min-w-36 p-0.5 flex border rounded border-gray-500" onClick={onClick}>
      {parts.map((part) =>
        part.amount > 0 ? (
          <div
            key={part.title}
            className="block h-full text-white text-xs text-center p-0.5 first:rounded-l last:rounded-r"
            style={{
              width: `calc(${part.amount} / ${totalCalc} * 100%)`,
              backgroundColor: part.color,
            }}
          >{part.amount}</div>
        ) : null,
      )}
    </div>
    <div className="ml-1">{total}</div>
    </div>
  );
};
