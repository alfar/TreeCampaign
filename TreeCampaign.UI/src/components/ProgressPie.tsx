interface ProgressSection {
  title: string;
  amount: number;
  color: string;
  textColor?: string;
}

interface ProgressPieProps {
  total?: number;
  parts: ProgressSection[];
}

export default ({ parts, total }: ProgressPieProps) => {
  const totalCalc =
    total ?? parts.reduce((prev, part) => part.amount + prev, 0);

  const percentages = parts.map((part) => ({
    ...part,
    percentage: (part.amount * 100.0) / totalCalc,
  }));

  const aggregate = percentages.reduce(
    (acc, part) => ({
      gradient:
        acc.gradient +
        ", " +
        part.color +
        " " +
        acc.accumulated +
        "%, " +
        part.color +
        " " +
        (acc.accumulated + part.percentage) +
        "%",
      accumulated: acc.accumulated + part.percentage,
      list: [...acc.list, { ...part, percenter: acc.accumulated + part.percentage / 2.0 }],
    }),
    { gradient: "", accumulated: 0, list: [] as { title: string; amount: number; percentage: number; percenter: number, color: string, textColor?: string }[] },
  );

  const gradient =
    aggregate.gradient.slice(2) +
    (aggregate.accumulated < 100
      ? ", #ffffff " + aggregate.accumulated + "%, #ffffff 100%"
      : ""); // remove leading ", "

  return (
    <div
      className="w-full aspect-square rounded-full border border-gray-500 relative"
      style={{ backgroundImage: `conic-gradient(${gradient})` }}
    >
      {aggregate.list.map((part) =>
        part.percentage > 4 ? (
          <div
            key={part.title}
            className="text-sm absolute"
            style={{
              color: part.textColor ?? "#ffffff",
              top:
                "calc(50% + " +
                Math.floor(Math.sin((part.percenter / 50.0) * Math.PI - Math.PI / 2) * 80) +
                "px)",
              left:
                "calc(50% + " +
                Math.floor(Math.cos((part.percenter / 50.0) * Math.PI - Math.PI / 2) * 80) +
                "px)",
            }}
          >
            <div style={{ margin: "-50% -50%" }}>{part.amount}</div>
          </div>
        ) : null,
      )}
    </div>
  );
};
